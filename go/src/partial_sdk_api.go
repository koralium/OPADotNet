package main

import "C"
import (
	"encoding/json"
	"sync"

	"github.com/open-policy-agent/opa/rego"
)

var partialQueryMutex sync.Mutex
var partialQueryToken int
var partialQueryMap sync.Map

//export PreparePartial
func PreparePartial(compilerId int, storeId int, query *C.char) C.int {
	queryData := C.GoString(query)
	compiler := getCompiler(compilerId)
	store := getStore(storeId)
	partialQuery, err := prepareForPartial(compiler, store, queryData)

	if err != nil {
		return addError(err.Error())
	}

	partialQueryMutex.Lock()
	partialQueryToken++
	i := partialQueryToken
	partialQueryMutex.Unlock()
	partialQueryMap.Store(i, partialQuery)
	return C.int(i)
}

//export RemovePartialQuery
func RemovePartialQuery(partialQueryId int) {
	partialQueryMap.Delete(partialQueryId)
}

func getPreparedPartial(partialQueryId int) rego.PreparedPartialQuery {
	pq, ok := partialQueryMap.Load(partialQueryId)

	if !ok {
		panic("could not find partial query")
	}
	return pq.(rego.PreparedPartialQuery)
}

//export PreparedPartial
func PreparedPartial(partialId int, input *C.char, unknowns **C.char, unknownsLength C.int, trace C.char) C.int {
	unknownsArray := GoStrings(unknownsLength, unknowns)

	useTrace := false
	if trace > 0 {
		useTrace = true
	}

	inputJson := C.GoString(input)
	var inputData interface{}
	json.Unmarshal([]byte(inputJson), &inputData)

	prepared := getPreparedPartial(partialId)

	pq, err := preparedPartial(prepared, inputData, unknownsArray, useTrace)

	if err != nil {
		return addError(err.Error())
	}

	bytes, err := json.Marshal(pq)

	if err != nil {
		return addError(err.Error())
	}

	return addString(string(bytes))
}

//export FullPartial
func FullPartial(compilerId int, storeId int, query *C.char, unknowns **C.char, unknownsLength C.int) C.int {
	queryData := C.GoString(query)
	compiler := getCompiler(compilerId)
	store := getStore(storeId)
	unknownsArray := GoStrings(unknownsLength, unknowns)

	pq, err := fullPartial(compiler, store, queryData, unknownsArray)

	if err != nil {
		return addError(err.Error())
	}

	bytes, err := json.Marshal(pq)

	if err != nil {
		return addError(err.Error())
	}

	return addString(string(bytes))
}
