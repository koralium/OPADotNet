package main

import "C"
import (
	"encoding/json"
	"errors"
	"sync"

	"github.com/open-policy-agent/opa/rego"
)

var evalQueryMutex sync.Mutex
var evalQueryToken int
var evalQueryMap sync.Map

//export PrepareEvaluation
func PrepareEvaluation(compilerId C.int, storeId C.int, query *C.char) C.int {
	queryData := C.GoString(query)
	compiler := getCompiler(int(compilerId))
	store := getStore(int(storeId))
	preparedQuery, err := prepareForEvaluation(compiler, store, queryData)

	if err != nil {
		return addError(err.Error())
	}

	evalQueryMutex.Lock()
	evalQueryToken++
	i := evalQueryToken
	evalQueryMutex.Unlock()
	evalQueryMap.Store(i, preparedQuery)
	return C.int(i)
}

//export RemoveEvalQuery
func RemoveEvalQuery(evalQueryId C.int) {
	evalQueryMap.Delete(int(evalQueryId))
}

func getPreparedEval(evalQueryId int) (rego.PreparedEvalQuery, error) {
	pq, ok := evalQueryMap.Load(evalQueryId)

	if !ok {
		return rego.PreparedEvalQuery{}, errors.New("Could not find prepared eval query")
	}

	return pq.(rego.PreparedEvalQuery), nil
}

//export PreparedEval
func PreparedEval(evalId C.int, input *C.char) C.int {
	inputJson := C.GoString(input)
	var inputData interface{}
	json.Unmarshal([]byte(inputJson), &inputData)

	prepared, err := getPreparedEval(int(evalId))

	if err != nil {
		return addError(err.Error())
	}

	rs, err := preparedEval(prepared, inputData)

	if err != nil {
		return addError(err.Error())
	}

	bytes, err := json.Marshal(rs)

	if err != nil {
		return addError(err.Error())
	}

	return addString(string(bytes))
}
