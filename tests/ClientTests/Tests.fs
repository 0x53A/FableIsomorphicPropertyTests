module ClientTests.Tests


open Fable.Tests.Util
open Fable.Tests.Util.Testing


type T =
#if FABLE_COMPILER
    static member test x = Fable.Core.Testing.Assert.AreEqual(true, x)
#else
    // https://github.com/SwensenSoftware/unquote/issues/142
    //static member test ([<ReflectedDefinition(true)>] x:Microsoft.FSharp.Quotations.Expr<bool>) = Swensen.Unquote.Assertions.test x
    static member test x = if not x then failwith "error"
#endif

let jsc : jsverify.JSVerify.IExports =
#if FABLE_COMPILER
    jsverify.jSVerify
#else
    Unchecked.defaultof<jsverify.JSVerify.IExports>
#endif
open Fable.Core.JsInterop


// -----------------------
// This is the code-under-test

// Sort by Words
module SortByWords =

    type private CompareResult = AThenB = -1 | BThenA = 1 | Equal = 0
    let private CompareResultFromInt i = if i = 0 then CompareResult.Equal else if i < 0 then CompareResult.AThenB else CompareResult.BThenA

    let sortByWordsComparer selector =
        fun a b ->
            let sA : string = selector a
            let sB : string = selector b
            if sA = null || sB = null then String.Compare(sA, sB) else

            let wordsA = sA.Split([|' '|], StringSplitOptions.RemoveEmptyEntries)
            let wordsB = sB.Split([|' '|], StringSplitOptions.RemoveEmptyEntries)

            let rec compare i =
              if i >= wordsA.Length && i >= wordsB.Length then CompareResult.Equal
              else if i >= wordsA.Length then CompareResult.AThenB
              else if i >= wordsB.Length then CompareResult.BThenA
              else
                let thisWordResult =
                    let wA = wordsA.[i]
                    let wB = wordsB.[i]
                    match Int32.TryParse(wA), Int32.TryParse(wB) with
                    | (true, iA), (true, iB) -> CompareResultFromInt (iA - iB)
                    | (true, _), _ -> CompareResult.AThenB
                    | _, (true, _) -> CompareResult.BThenA
                    | (false, _), (false, _) -> CompareResultFromInt ( wA.CompareTo(wB) )
                if thisWordResult <> CompareResult.Equal then thisWordResult else compare (i+1)
            int (compare 0)


    let sortByWords (selector:'a->string) (input:#seq<'a>) : seq<'a> =
        let list = System.Collections.Generic.List(input)
        list.Sort(sortByWordsComparer selector)
        list :> _


// -----------------------------



let wordSortTests =
  testList "Sort By Words" [
    testList "nested test list" [
      testCase "testCase" (fun _ ->
        let input = [
            "TP 1"
            "TP 10"
            "A"
            "TP 2"
        ]
        let output =
            input
            |> SortByWords.sortByWords id
            |> Seq.toList

        let expected = [
            "A"
            "TP 1"
            "TP 2"
            "TP 10"
        ]

        T.test (( expected = output ))
    )]
    
    testProperty "comparer is kommutativ - order doesn't matter" (fun fn -> jsc.forall(jsc.string, jsc.string, (fun b1 b2 -> !^ fn(b1,b2) ))) (fun (s1, s2) ->
        let comparer = ebosYC.Client.SortByWords.sortByWordsComparer id
        (comparer s1 s2) = (-1*(comparer s2 s1))
    )
  ]

let clientTests = [
    wordSortTests
  ]

