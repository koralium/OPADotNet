package main

// typedef void (*evaluate_cb)(char*);
// extern void callEvaluateCallback(char* rs, evaluate_cb cb) {
//   cb(rs);
// }
// typedef void (*pointer_cb)(int, char*);
// extern void callPointerCallback(int pointer, char* err, pointer_cb cb) {
//   cb(pointer, err);
// }
import "C"

import (
	"context"

	"github.com/open-policy-agent/opa/ast"
	"github.com/open-policy-agent/opa/rego"
	"github.com/open-policy-agent/opa/storage"
)

func compileModules(modules map[string]string) (*ast.Compiler, error) {
	return ast.CompileModules(modules)
}

func getModules(compiler *ast.Compiler) map[string]*ast.Module {
	return compiler.Modules
}

type evaluateCb func(rego.ResultSet)

func evaluate(compiler *ast.Compiler, query string, input interface{}, callback evaluateCb) {
	ctx := context.Background()
	rego := rego.New(
		rego.Query(query),
		rego.Compiler(compiler),
		rego.Input(input),
	)

	rs, err := rego.Eval(ctx)

	if err != nil {
		// Handle error.
	}

	callback(rs)
}

type partialCb func(*rego.PartialQueries)

func prepareForPartial(compiler *ast.Compiler, store storage.Store, query string) (rego.PreparedPartialQuery, error) {
	ctx := context.Background()

	r := rego.New(
		rego.Query(query),
		rego.Compiler(compiler),
		rego.Store(store),
	)

	return r.PrepareForPartial(ctx)
}

func preparedPartial(prepared rego.PreparedPartialQuery, input interface{}, unknowns []string) (*rego.PartialQueries, error) {
	ctx := context.Background()
	return prepared.Partial(ctx, rego.EvalInput(input), rego.EvalUnknowns(unknowns))
}

func partial(compiler *ast.Compiler, query string, input interface{}, callback partialCb) {
	ctx := context.Background()

	r := rego.New(
		rego.Query(query),
		rego.Compiler(compiler),
		//rego.Input(input),
	)

	pq, err := r.PrepareForPartial(ctx)
	if err != nil {
		// Handle error.
	}

	pqs, err := pq.Partial(ctx, rego.EvalInput(input), rego.EvalUnknowns([]string{}))

	if err != nil {
		// Handle error
	}

	callback(pqs)
}
