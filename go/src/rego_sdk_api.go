package main

//#include <stdlib.h>
// typedef void (*evaluate_cb)(char*);
// void callEvaluateCallback(char* rs, evaluate_cb cb);
// typedef void (*pointer_cb)(int, char*);
// void callPointerCallback(int pointer, char* err, pointer_cb cb);
import "C"
import (
	"encoding/json"
	"sync"
	"unsafe"

	"github.com/open-policy-agent/opa/ast"
	"github.com/open-policy-agent/opa/rego"
)

var compilerMutex sync.Mutex
var compilerToken int
var compilerMap sync.Map

//export RemoveCompiler
func RemoveCompiler(compilerId int) {
	compilerMap.Delete(compilerId)
}

func getCompiler(id int) *ast.Compiler {
	compiler, ok := compilerMap.Load(id)

	if !ok {
		panic("could not find compiler")
	}
	return compiler.(*ast.Compiler)
}

//export CompileModules
func CompileModules(modules *C.char) C.int {
	modulesJson := C.GoString(modules)

	var moduleData map[string]string
	err := json.Unmarshal([]byte(modulesJson), &moduleData)

	if err != nil {
		return addError(err.Error())
	}

	compiler, err := compileModules(moduleData)

	if err != nil {
		return addError(err.Error())
	}

	compilerMutex.Lock()
	compilerToken++
	i := compilerToken
	compilerMutex.Unlock()
	compilerMap.Store(i, compiler)
	return C.int(i)
}

//export GetModules
func GetModules(compilerId C.int) C.int {
	compiler := getCompiler(int(compilerId))
	modules := getModules(compiler)

	bytes, err := json.Marshal(modules)

	if err != nil {
		return addError(err.Error())
	}

	return addString(string(bytes))
}

//export Evaluate
func Evaluate(compilerId int, query *C.char, input *C.char, callback C.evaluate_cb) {
	inputJson := C.GoString(input)
	queryData := C.GoString(query)

	var inputData interface{}
	json.Unmarshal([]byte(inputJson), &inputData)

	goCb := func(rs rego.ResultSet) {
		bytes, err := json.Marshal(rs)

		if err != nil {
			//TODO
		}

		//Allocate memory for the string
		data := C.CString(string(bytes))

		C.callEvaluateCallback(data, callback)
		//Free allocated memory
		C.free(unsafe.Pointer(data))
	}

	compiler := getCompiler(compilerId)
	evaluate(compiler, queryData, inputData, goCb)
}

//export Partial
func Partial(compilerId int, query *C.char, input *C.char, callback C.evaluate_cb) {
	inputJson := C.GoString(input)
	queryData := C.GoString(query)

	var inputData interface{}
	json.Unmarshal([]byte(inputJson), &inputData)

	goCb := func(rs *rego.PartialQueries) {
		bytes, err := json.Marshal(rs)

		if err != nil {
			//TODO
		}

		//Allocate memory for the string
		data := C.CString(string(bytes))

		C.callEvaluateCallback(data, callback)
		//Free allocated memory
		C.free(unsafe.Pointer(data))
	}

	compiler := getCompiler(compilerId)
	partial(compiler, queryData, inputData, goCb)
}

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
		// errorMessage := C.CString(err.Error())
		// C.callPointerCallback(C.int(-1), errorMessage, callback)
		// C.free(unsafe.Pointer(errorMessage))
		return addError(err.Error())
	}

	partialQueryMutex.Lock()
	partialQueryToken++
	i := partialQueryToken
	partialQueryMutex.Unlock()
	partialQueryMap.Store(i, partialQuery)
	return C.int(i)
	// C.callPointerCallback(C.int(i), emptyError, callback)
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
func PreparedPartial(partialId int, input *C.char, unknowns **C.char, unknownsLength C.int) C.int {
	unknownsArray := GoStrings(unknownsLength, unknowns)

	inputJson := C.GoString(input)
	var inputData interface{}
	json.Unmarshal([]byte(inputJson), &inputData)

	prepared := getPreparedPartial(partialId)

	// goCb := func(rs *rego.PartialQueries) {
	// 	bytes, err := json.Marshal(rs)

	// 	if err != nil {
	// 		//TODO
	// 	}

	// 	//Allocate memory for the string
	// 	data := C.CString(string(bytes))

	// 	C.callEvaluateCallback(data, callback)
	// 	//Free allocated memory
	// 	C.free(unsafe.Pointer(data))
	// }

	pq, err := preparedPartial(prepared, inputData, unknownsArray)

	if err != nil {
		return addError(err.Error())
	}

	bytes, err := json.Marshal(pq)

	if err != nil {
		return addError(err.Error())
	}

	return addString(string(bytes))
}

func GoStrings(argc C.int, argv **C.char) []string {

	length := int(argc)
	tmpslice := (*[1 << 28]*C.char)(unsafe.Pointer(argv))[:length:length]
	gostrings := make([]string, length)
	for i, s := range tmpslice {
		gostrings[i] = C.GoString(s)
	}
	return gostrings
}

func main() {

}
