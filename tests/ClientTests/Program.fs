module ClientTests.TestsRunner

let allTests = ClientTests.Tests.clientTests

#if FABLE_COMPILER

open Fable.Core
open Fable.Core.JsInterop

// Import a polyfill for atob and btoa, used by fable-core
// but not available in node.js runtime
importSideEffects "./js/polyfill"

let [<Global>] describe (name: string) (f: unit->unit) : unit = jsNative
let [<Global>] it (msg: string) (f: unit->unit) : unit = jsNative


let jsc = jsverify.jSVerify

let rec flattenTest (test:Fable.Tests.Util.Testing.TestKind) : unit =
    match test with
    | Fable.Tests.Util.Testing.TestKind.TestList(name, tests) ->
        describe name (fun () ->
          for t in tests do
            flattenTest t)
    | Fable.Tests.Util.Testing.TestKind.TestCase (name, test) ->
        it name (unbox test)
    | Fable.Tests.Util.Testing.TestKind.TestProperty(name, js, test) ->
        it name (fun () -> jsc.``assert``(js(test)))


let run () =
    for t in allTests do
        flattenTest t
run()

#else

open System
open Expecto

let (@@) a b = System.IO.Path.Combine(a, b)

[<EntryPoint>]
let main args =
    let config = { defaultConfig with ``parallel`` = false }
    runTestsWithArgs config  args (testList "ClientTests" allTests)

#endif