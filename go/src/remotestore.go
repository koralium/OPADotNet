package main

// typedef char* (*read_cb)(char*);
// extern char* callReadCallback(char* path, read_cb cb) {
//   return cb(path);
// }
import "C"

import (
	"context"
	"encoding/json"
	"strings"
	"sync"

	"github.com/open-policy-agent/opa/storage"
	"github.com/open-policy-agent/opa/storage/inmem"
)

type Store struct {
	inmem        storage.Store
	baseData     map[string]interface{}
	Transactions []*storage.Transaction
	rootNode     *PathNode
}

type PathNode struct {
	children sync.Map
	leaf     bool
	callback C.read_cb
}

func (s *Store) readFromRemote(txn storage.Transaction, path storage.Path) (interface{}, bool, error) {

	currentNode := s.rootNode
	for i := 0; i < len(path); i++ {
		// Get the node in the path
		node, ok := currentNode.children.Load(path[i])

		// If there is no more node in the path, break
		if !ok {
			break
		}

		//Set the new node as current
		currentNode = node.(*PathNode)
	}

	// If the last node visited was not a leaf node
	if !currentNode.leaf {
		return nil, false, nil
	}

	returnDataC := C.callReadCallback(C.CString(strings.Join(path, "/")), currentNode.callback)
	returnData := C.GoString(returnDataC)
	var result interface{}
	jsonError := json.Unmarshal([]byte(returnData), &result)

	if jsonError != nil {
		return nil, true, jsonError
	}

	return result, true, nil
}

func (s *Store) registerRemoteStore(path string, callback C.read_cb) {
	// Slip the path by '/'
	// Treat the remote map as a tree of maps
	split := strings.Split(path, "/")

	currentNode := s.rootNode

	// Iterate the first part of the path and create nodes in the tree
	for i := 0; i < len(split)-1; i++ {
		node, ok := currentNode.children.Load(split[i])

		childNode := node.(*PathNode)
		if !ok {
			childNode = &PathNode{}
			currentNode.children.Store(split[i], childNode)
		}
		// Set the current node to the child node
		currentNode = childNode
	}

	// Set the leaf node
	currentNode.children.Store(split[len(split)-1], &PathNode{
		leaf:     true,
		callback: callback,
	})
}

func NewRemote() *Store {
	s := &Store{}
	s.inmem = inmem.New()
	s.rootNode = &PathNode{}
	return s
}

func (s *Store) NewTransaction(ctx context.Context, params ...storage.TransactionParams) (storage.Transaction, error) {
	return s.inmem.NewTransaction(ctx, params...)
}

// Register just shims the call to the underlying inmem store
func (s *Store) Register(ctx context.Context, txn storage.Transaction, config storage.TriggerConfig) (storage.TriggerHandle, error) {
	return s.inmem.Register(ctx, txn, config)
}

// ListPolicies just shims the call to the underlying inmem store
func (s *Store) ListPolicies(ctx context.Context, txn storage.Transaction) ([]string, error) {
	return s.inmem.ListPolicies(ctx, txn)
}

// GetPolicy just shims the call to the underlying inmem store
func (s *Store) GetPolicy(ctx context.Context, txn storage.Transaction, name string) ([]byte, error) {
	return s.inmem.GetPolicy(ctx, txn, name)
}

// UpsertPolicy just shims the call to the underlying inmem store
func (s *Store) UpsertPolicy(ctx context.Context, txn storage.Transaction, name string, policy []byte) error {
	return s.inmem.UpsertPolicy(ctx, txn, name, policy)
}

// DeletePolicy just shims the call to the underlying inmem store
func (s *Store) DeletePolicy(ctx context.Context, txn storage.Transaction, name string) error {
	return s.inmem.DeletePolicy(ctx, txn, name)
}

func (s *Store) Read(ctx context.Context, txn storage.Transaction, path storage.Path) (interface{}, error) {
	remoteData, hasRead, err := s.readFromRemote(txn, path)

	if hasRead {
		if err != nil {
			return nil, err
		}
		return remoteData, nil
	}

	data, err := s.inmem.Read(ctx, txn, path)
	return data, err
}

func (s *Store) Write(ctx context.Context, txn storage.Transaction, op storage.PatchOp, path storage.Path, value interface{}) error {
	err := s.inmem.Write(ctx, txn, op, path, value)
	return err
}

func (s *Store) Commit(ctx context.Context, txn storage.Transaction) error {
	err := s.inmem.Commit(ctx, txn)
	return err
}

func (s *Store) Abort(ctx context.Context, txn storage.Transaction) {
	s.inmem.Abort(ctx, txn)
}
