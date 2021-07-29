package main

//#include <stdlib.h>
// typedef void (*transaction_cb)(int, char*);
// void callTransactionCallback(int transactionId, char* err, transaction_cb cb);
// typedef void (*error_cb)(char*);
// void callErrorCallback(char* err, error_cb cb);
import "C"
import (
	"encoding/json"
	"sync"

	"github.com/open-policy-agent/opa/storage"
)

var storeMutex sync.Mutex
var storeToken int
var storeMap sync.Map

var emptyError *C.char = C.CString("")

//export NewStore
func NewStore() int {
	storeMutex.Lock()
	storeToken++
	i := storeToken
	storeMutex.Unlock()
	storeMap.Store(i, newStore())
	return i
}

//export RemoveStore
func RemoveStore(storeId int) {
	storeMap.Delete(storeId)
}

//export WriteToStore
func WriteToStore(storeId C.int, transactionId C.int, path *C.char, input *C.char) {
	pathString := C.GoString(path)
	inputJson := C.GoString(input)

	var inputData interface{}
	json.Unmarshal([]byte(inputJson), &inputData)
	store := getStore(int(storeId))
	txn := getTransaction(int(transactionId))
	writeToStore(store, txn, pathString, inputData)
}

func getStore(storeId int) storage.Store {
	store, ok := storeMap.Load(storeId)

	if !ok {
		panic("could not find compiler")
	}
	return store.(storage.Store)
}

var transactionMutex sync.Mutex
var transactionToken int
var transactionMap sync.Map

//export NewTransaction
func NewTransaction(storeId C.int) C.int {
	id := int(storeId)
	store := getStore(id)

	txn, err := newTransaction(store)

	if err != nil {
		//errorMessage := C.CString(err.Error())
		//C.callTransactionCallback(C.int(-1), errorMessage, callback)
		//defer C.free(unsafe.Pointer(errorMessage))
		return -1
	}

	transactionMutex.Lock()
	transactionToken++
	i := transactionToken
	transactionMutex.Unlock()
	transactionMap.Store(i, txn)
	//C.callTransactionCallback(C.int(i), emptyError, callback)
	return C.int(i)
}

//export RemoveTransaction
func RemoveTransaction(transactionId C.int) {
	transactionMap.Delete(int(transactionId))
}

func getTransaction(transactionId int) storage.Transaction {
	txn, ok := transactionMap.Load(transactionId)

	if !ok {
		panic("could not find transaction")
	}
	return txn.(storage.Transaction)
}

//export CommitTransaction
func CommitTransaction(storeId C.int, transactionId C.int) C.int {
	txn := getTransaction(int(transactionId))
	store := getStore(int(storeId))

	err := commitTransaction(store, txn)

	if err != nil {
		//errorMessage := C.CString(err.Error())
		//C.callErrorCallback(errorMessage, callback)
		//C.free(unsafe.Pointer(errorMessage))
		return addError(err.Error())
	}
	return 0
	//C.callErrorCallback(emptyError, callback)
}

//export UpsertPolicy
func UpsertPolicy(storeId C.int, transactionId C.int, policyName *C.char, module *C.char, callback C.error_cb) int {
	store := getStore(int(storeId))
	txn := getTransaction(int(transactionId))

	goPolicyName := C.GoString(policyName)
	goModule := C.GoString(module)
	err := upsertPolicy(store, txn, goPolicyName, goModule)

	if err != nil {
		//errorMessage := C.CString(err.Error())
		//C.callErrorCallback(errorMessage, callback)
		//C.free(unsafe.Pointer(errorMessage))
		return -1
	}
	return 0
	//C.callErrorCallback(emptyError, callback)
}

//export ReadFromStore
func ReadFromStore(storeId C.int, transactionId C.int, path *C.char) C.int {
	store := getStore(int(storeId))
	txn := getTransaction(int(transactionId))
	pathString := C.GoString(path)

	result, err := readFromStore(store, txn, pathString)

	if err != nil {
		return addError(err.Error())
	}

	bytes, err := json.Marshal(result)

	if err != nil {
		return addError(err.Error())
	}

	return addString(string(bytes))
}
