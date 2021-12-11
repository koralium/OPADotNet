package main

// typedef char* (*function_cb)(char*);
// extern char* callFunctionCallback(char* json, function_cb cb) {
//   return cb(json);
// }
import "C"

import (
	"encoding/json"

	"github.com/open-policy-agent/opa/ast"
	"github.com/open-policy-agent/opa/rego"
	"github.com/open-policy-agent/opa/types"
)

func registerFunction1(name string, callfunc C.function_cb) {
	rego.RegisterBuiltin1(
		&rego.Function{
			Name:    name,
			Decl:    types.NewFunction(types.Args(types.A), types.A),
			Memoize: true,
		},
		func(bctx rego.BuiltinContext, a *ast.Term) (*ast.Term, error) {
			bytes, err := json.Marshal(a)

			if err != nil {
				return nil, err
			}

			returnVal := C.GoString(C.callFunctionCallback(C.CString(string(bytes)), callfunc))
			var inputData *ast.Term
			json.Unmarshal([]byte(returnVal), &inputData)
			return inputData, nil
		},
	)
}
