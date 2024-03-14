#r "nuget: Microsoft.Z3, 4.11.2"

open Microsoft.Z3

let ctx = new Context()
let solver = ctx.MkSolver()

// ----------------------------------------------------------------------------
// Array sort

// Arrays are more akin to maps: They have a domain and a range.
// An array in the more classical sense is an Int * 'a array.

let intArraySort = ctx.MkArraySort(ctx.IntSort, ctx.IntSort)

let ia1: ArrayExpr = ctx.MkFreshConst("ia1", intArraySort) :?> ArrayExpr

solver.Reset()

// Making the array [| 0; 1 |]
solver.Add(
    [| ctx.MkEq(ctx.MkSelect(ia1, ctx.MkInt(0)), ctx.MkInt(3))
       ctx.MkEq(ctx.MkSelect(ia1, ctx.MkInt(1)), ctx.MkInt(7)) |]
)

solver.Check()
solver.Model.Eval(ctx.MkSelect(ia1, ctx.MkInt(0)), true).ToString() // "3"
solver.Model.Eval(ctx.MkSelect(ia1, ctx.MkInt(1)), true).ToString() // "7"

// NOTE Because arrays are just mappings, the rest of the domain maps to the
// last assignment, in this case 7. 
solver.Model.Eval(ia1, true).ToString() // "(store ((as const (Array Int Int)) 7) 0 3)"
solver.Model.Eval(ctx.MkSelect(ia1, ctx.MkInt(2)), true).ToString() // "7"
solver.Model.Eval(ctx.MkSelect(ia1, ctx.MkInt(-1)), true).ToString() // "7"

// TODO Try using string or bool as domain
//
