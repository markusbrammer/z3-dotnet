#r "nuget: Microsoft.Z3, 4.11.2"

open Microsoft.Z3

let ctx = new Context()
let solver = ctx.MkSolver()

// ----------------------------------------------------------------------------
// Making tree sort with empty and binary node

let mutable treeSort: DatatypeSort = null

let emptyNodeCtor = ctx.MkConstructor("EmptyNode", null, null, null) // Might have to replace null with [| |]

let binNodeSorts: Sort array = [| treeSort; ctx.StringSort; treeSort |]

let binNodeCtor =
    ctx.MkConstructor("BinNode", "IsBinNode", [| "left"; "label"; "right" |], binNodeSorts)

treeSort <- ctx.MkDatatypeSort("Tree", [| emptyNodeCtor; binNodeCtor |])

// // Ensuring that the sort is recursive
// match treeSort.Constructors.[1].Domain.[0] with
// | :? DatatypeSort as t -> t.Constructors.[0]

let mkEmpty () =
    let emptyNodeDecl = treeSort.Constructors.[0]

    emptyNodeDecl.Apply()

let mkBin l s r =
    let binNodeDecl = treeSort.Constructors.[1]
    let args: Expr array = [| l; ctx.MkString(s); r |]

    binNodeDecl.Apply(args)

let getLeft binNode = 
    binNodeCtor.AccessorDecls.[0].Apply([| binNode |])

let getRight binNode = 
    binNodeCtor.AccessorDecls.[2].Apply([| binNode |])

let getLabel binNode = 
    binNodeCtor.AccessorDecls.[1].Apply([| binNode |])

// ----------------------------------------------------------------------------
// An empty node and a binary node are not the same

let t1 = mkEmpty ()
let t2 = mkBin t1 "a" t1

solver.Add(ctx.MkEq(t1, t2))
solver.Check() // unsat
solver.Reset()

// ----------------------------------------------------------------------------
// Two empty nodes are the same

let t3 = mkEmpty ()
let t4 = mkEmpty ()

solver.Add(ctx.MkEq(t3, t4))
solver.Check() // sat
solver.Reset()

// ----------------------------------------------------------------------------
// Two identical binary nodes are the same

let t5 = mkBin t1 "a" t1
let t6 = mkBin t1 "a" t1

solver.Add(ctx.MkEq(t5, t6))
solver.Check() // sat
solver.Reset()

// ----------------------------------------------------------------------------
// Two non-identical binary nodes are NOT the same

let t7 = mkBin t1 "a" t1
let t8 = mkBin t1 "b" t1

solver.Add(ctx.MkEq(t7, t8))
solver.Check() // unsat
solver.Reset()

// ----------------------------------------------------------------------------
// Testing getters/accessors

let t9 = mkBin t7 "c" t8

// Could also use .ToString() but the fields have a field String
(getLabel t9).Simplify().String // c
(getLeft t9 |> getLabel).Simplify().String // a
(getRight t9 |> getLabel).Simplify().String // b

