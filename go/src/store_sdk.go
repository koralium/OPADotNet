package main

// typedef void (*transaction_cb)(int, char*);
// extern void callTransactionCallback(int transactionId, char* err, transaction_cb cb) {
//   cb(transactionId, err);
// }
// typedef void (*error_cb)(char*);
// extern void callErrorCallback(char* err, error_cb cb) {
//   cb(err);
// }
import "C"

import (
	"context"

	"github.com/open-policy-agent/opa/storage"
	"github.com/open-policy-agent/opa/storage/inmem"
)

func newStore() storage.Store {
	return inmem.New()
}

func newTransaction(store storage.Store) (storage.Transaction, error) {
	ctx := context.Background()
	return store.NewTransaction(ctx, storage.WriteParams)
}

func writeToStore(store storage.Store, txn storage.Transaction, path string, input interface{}) {
	ctx := context.Background()

	if err := store.Write(ctx, txn, storage.AddOp, storage.MustParsePath(path), input); err != nil {
		panic(err)
	}
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
