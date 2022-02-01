package main

import (
	"bytes"
	"encoding/json"
	"strings"

	"github.com/open-policy-agent/opa/ast"
	"github.com/open-policy-agent/opa/rego"
	"github.com/open-policy-agent/opa/topdown"
)

type TraceEventV1 struct {
	Op       string      `json:"op"`
	QueryID  uint64      `json:"query_id"`
	ParentID uint64      `json:"parent_id"`
	Type     string      `json:"type"`
	Node     interface{} `json:"node"`
	Locals   BindingsV1  `json:"locals"`
	Location LocationV1  `json:"location"`
	Message  string      `json:"message,omitempty"`
}

// BindingsV1 represents a set of term bindings.
type BindingsV1 []*BindingV1

// BindingV1 represents a single term binding.
type BindingV1 struct {
	Key   *ast.Term `json:"key"`
	Value *ast.Term `json:"value"`
}

type LocationV1 struct {
	File string `json:"file"`
	Row  int    `json:"row"`
}

type TraceV1Raw []TraceEventV1

type TraceV1 json.RawMessage

type PartialResponse struct {
	Result      *rego.PartialQueries `json:"result,omitempty"`
	Explanation TraceV1Raw           `json:"explanation,omitempty"`
}

func newPrettyTraceV1(trace []*topdown.Event) (TraceV1, error) {
	var buf bytes.Buffer
	topdown.PrettyTraceWithLocation(&buf, trace)

	str := strings.Trim(buf.String(), "\n")
	b, err := json.Marshal(strings.Split(str, "\n"))
	if err != nil {
		return nil, err
	}
	return TraceV1(json.RawMessage(b)), nil
}

func NewLocationV1(loc *ast.Location) LocationV1 {

	if loc == nil {
		return LocationV1{
			File: "",
			Row:  -1,
		}
	}

	fileName := "query"

	if loc.File != "" {
		fileName = loc.File
	}

	return LocationV1{
		File: fileName,
		Row:  loc.Row,
	}
}

func newRawTraceV1(trace []*topdown.Event) (TraceV1Raw, error) {
	result := TraceV1Raw(make([]TraceEventV1, len(trace)))
	for i := range trace {

		result[i] = TraceEventV1{
			Op:       strings.ToLower(string(trace[i].Op)),
			QueryID:  trace[i].QueryID,
			ParentID: trace[i].ParentID,
			Locals:   NewBindingsV1(trace[i].Locals),
			Message:  trace[i].Message,
			Location: NewLocationV1(trace[i].Location),
		}
		if trace[i].Node != nil {
			result[i].Type = ast.TypeName(trace[i].Node)
			result[i].Node = trace[i].Node
		}
	}

	return result, nil
}

func NewBindingsV1(locals *ast.ValueMap) (result []*BindingV1) {
	result = make([]*BindingV1, 0, locals.Len())
	locals.Iter(func(key, value ast.Value) bool {
		result = append(result, &BindingV1{
			Key:   &ast.Term{Value: key},
			Value: &ast.Term{Value: value},
		})
		return false
	})
	return result
}
