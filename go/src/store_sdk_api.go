package main

//#include <stdlib.h>
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
func WriteToStore(storeId C.int, transactionId C.int, path *C.char, input *C.char) C.int {
	pathString := C.GoString(path)
	inputJson := C.GoString(input)

	var inputData interface{}
	jsonError := json.Unmarshal([]byte(inputJson), &inputData)

	if jsonError != nil {
		return addError(jsonError.Error())
	}

	store := getStore(int(storeId))
	txn := getTransaction(int(transactionId))
	err := writeToStore(store, txn, pathString, inputData)

	if err != nil {
		return addError(err.Error())
	}
	return C.int(0)
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

func intToBool(b C.int) bool {
	i := int(b)

	if i == 0 {
		return false
	} else {
		return true
	}
}

//export NewTransaction
func NewTransaction(storeId C.int, write C.int) C.int {
	id := int(storeId)
	store := getStore(id)

	txn, err := newTransaction(store, intToBool(write))

	if err != nil {
		return addError(err.Error())
	}

	transactionMutex.Lock()
	transactionToken++
	i := transactionToken
	transactionMutex.Unlock()
	transactionMap.Store(i, txn)
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
		return addError(err.Error())
	}
	return 0
}

//export UpsertPolicy
func UpsertPolicy(storeId C.int, transactionId C.int, policyName *C.char, module *C.char) int {
	store := getStore(int(storeId))
	txn := getTransaction(int(transactionId))

	goPolicyName := C.GoString(policyName)
	goModule := C.GoString(module)
	err := upsertPolicy(store, txn, goPolicyName, goModule)

	if err != nil {
		return int(addError(err.Error()))
	}
	return 0
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
