#r "nuget: Microsoft.Z3, 4.11.2"

open Microsoft.Z3

let ctx = new Context()
let solver = ctx.MkSolver()

// ----------------------------------------------------------------------------
// Making tuple sort

let mkTupleSort (name: string) fieldNames fieldSorts =
    let nameSym: Symbol = ctx.MkSymbol(name)

    let fieldNamesSyms: Symbol array =
        Array.map (fun (s: string) -> ctx.MkSymbol(s)) fieldNames

    ctx.MkTupleSort(nameSym, fieldNamesSyms, fieldSorts)

let infoSort =
    mkTupleSort "Info" [| "Name"; "Age" |] [| ctx.StringSort; ctx.IntSort |]

let mkInfo name age =
    let args: Expr array = [| ctx.MkString(name); ctx.MkInt(uint64 age) |]

    infoSort.MkDecl.Apply(args)

let getName info =
    infoSort.FieldDecls.[0].Apply([| info |])

let getAge info =
    infoSort.FieldDecls.[1].Apply([| info |])

// ----------------------------------------------------------------------------
// Creating/making a tuple and getting field values

let i0 = mkInfo "Markus" 25

(getName i0).Simplify().ToString() // ""Markus""
(getAge i0).Simplify().ToString() // "25"

i0.Sort.Name.ToString()

