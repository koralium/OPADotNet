package main

import (
	"context"

	"github.com/open-policy-agent/opa/storage"
	"github.com/open-policy-agent/opa/storage/inmem"
)

func newStore() storage.Store {
	return inmem.New()
}

func newTransaction(store storage.Store, write bool) (storage.Transaction, error) {
	ctx := context.Background()

	if write {
		return store.NewTransaction(ctx, storage.WriteParams)
	} else {
		return store.NewTransaction(ctx)
	}
}

func writeToStore(store storage.Store, txn storage.Transaction, path string, input interface{}) error {
	ctx := context.Background()

	p := storage.MustParsePath(path)
	storage.MakeDir(ctx, store, txn, p)
	return store.Write(ctx, txn, storage.AddOp, p, input)
}

func commitTransaction(store storage.Store, txn storage.Transaction) error {
	ctx := context.Background()
	return store.Commit(ctx, txn)
}

func upsertPolicy(store storage.Store, txn storage.Transaction, policyName string, policyData string) error {
	ctx := context.Background()
	return store.UpsertPolicy(ctx, txn, policyName, []byte(policyData))
}

func readFromStore(store storage.Store, txn storage.Transaction, path string) (interface{}, error) {
	ctx := context.Background()
	return store.Read(ctx, txn, storage.MustParsePath(path))
}
