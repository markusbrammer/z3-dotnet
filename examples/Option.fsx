#r "nuget: Microsoft.Z3, 4.11.2"

open Microsoft.Z3

let ctx = new Context()
let solver = ctx.MkSolver()
let model = solver.Model

// ============================================================================
// OptionSort
// ============================================================================

let mkOptionSort (dataSort: Sort) : DatatypeSort =
    let noneCtor: Constructor = ctx.MkConstructor("None", "IsNone")

    let someCtor dataSort = ctx.MkConstructor("Some", "IsSome", [| "Data" |], [| dataSort |])

    ctx.MkDatatypeSort("Option", [| noneCtor; someCtor dataSort |])

// ----------------------------------------------------------------------------
// Constructors

let mkNone (optionsSort: DatatypeSort) = 
    optionsSort.Constructors.[0].Apply()

let mkSome (optionsSort: DatatypeSort) (data: Expr) : Expr = 
    optionsSort.Constructors.[1].Apply(data)

// ----------------------------------------------------------------------------
// Recognizers

let isNone (optionsSort: DatatypeSort) (o: Expr) : BoolExpr = 
    optionsSort.Recognizers.[0].Apply(o) :?> BoolExpr

let isSome (optionsSort: DatatypeSort) (o: Expr) : BoolExpr = 
    optionsSort.Recognizers.[1].Apply(o) :?> BoolExpr

// ----------------------------------------------------------------------------
// Getters/Accessors

let getData (optionsSort: DatatypeSort) (o: Expr) : Expr = 
    optionsSort.Accessors.[1].[0].Apply(o)

// ============================================================================
// Examples
// ============================================================================

// ----------------------------------------------------------------------------
// Examples with Some

let intOptSort = mkOptionSort ctx.IntSort

let s33 = mkSome (intOptSort) (ctx.MkInt(33))

(isSome intOptSort s33).Simplify().ToString() // "true"

(isSome intOptSort s33).IsTrue // false

(isSome intOptSort s33).BoolValue // Z3_L_UNDEF

(isSome intOptSort s33).Simplify().IsTrue // true

(isSome intOptSort s33).Simplify().BoolValue // Z3_L_TRUE

(isNone intOptSort s33).Simplify().IsTrue // false

(getData intOptSort s33).Simplify() // 33 (IntSort)

(getData intOptSort s33).Simplify().ToString() // "33"

// ----------------------------------------------------------------------------
// Examples with None

let n = mkNone intOptSort

(isNone intOptSort n).Simplify().ToString() // "true"

(isSome intOptSort n).Simplify().ToString() // "false"

(getData intOptSort n).Simplify().ToString() // "(Data None)"

// ----------------------------------------------------------------------------
// Example using Solver and Eval

let sUnknown = ctx.MkFreshConst("sUnknown", intOptSort) 

solver.Reset(); solver.Check(ctx.MkEq(s33, sUnknown))

let sKnown = model.Eval(sUnknown, true) 

sKnown.ToString() // "(Some 33)"

