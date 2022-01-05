package main

import (
	"context"

	"github.com/open-policy-agent/opa/ast"
	"github.com/open-policy-agent/opa/rego"
	"github.com/open-policy-agent/opa/storage"
	"github.com/open-policy-agent/opa/topdown"
)

func prepareForPartial(compiler *ast.Compiler, store storage.Store, query string) (rego.PreparedPartialQuery, error) {
	ctx := context.Background()

	r := rego.New(
		rego.Query(query),
		rego.Compiler(compiler),
		rego.Store(store),
	)

	return r.PrepareForPartial(ctx)
}

func preparedPartial(prepared rego.PreparedPartialQuery, input interface{}, unknowns []string, trace bool) (PartialResponse, error) {
	ctx := context.Background()

	evalOpts := []rego.EvalOption{
		rego.EvalInput(input),
		rego.EvalUnknowns(unknowns),
	}

	if trace {
		var tracer = topdown.NewBufferTracer()
		evalOpts = append(evalOpts, rego.EvalQueryTracer(tracer))

		partialResult, err := prepared.Partial(ctx, evalOpts...)

		expl, err := newRawTraceV1(*tracer)
		res := PartialResponse{
			Result:      partialResult,
			Explanation: expl,
		}
		return res, err
	}

	partialResult, err := prepared.Partial(ctx, evalOpts...)

	res := PartialResponse{
		Result: partialResult,
	}
	return res, err
}
