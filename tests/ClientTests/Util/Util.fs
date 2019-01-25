module Fable.Tests.Util

open System

module Testing =

#if FABLE_COMPILER
    type TestKind =
    | TestList of string * TestKind seq
    | TestCase of (string*obj)
    | TestProperty of name:string * jsWrapper:((obj->bool)->jsverify.JSVerify.Property<unit>) * prop:(obj->bool)

    open Fable.Core
    open Fable.Core.Testing

    let testList (name: string) (tests: TestKind seq) = TestList( name, tests )
    let testCase (msg: string) (test: unit->unit) = TestCase( msg, box test )
    let testCaseAsync (msg: string) (test: unit->Async<unit>) = TestCase( msg, box(fun () -> test () |> Async.StartAsPromise) )
    let testProperty<'args> name (jsWrapper:('args->bool)->jsverify.JSVerify.Property<unit>) (test:'args->bool) = TestProperty( name, unbox jsWrapper, unbox test)

    let equal expected actual: unit = Assert.AreEqual(actual, expected)
    let notEqual expected actual: unit = Assert.AreEqual(false, (actual=expected))
#else
    open Expecto

    let testList name tests = testList name tests
    let testCase msg test = testCase msg test
    let testCaseAsync msg test = testCaseAsync msg (test ())
    let testProperty<'args> name (jsWrapper:('args->bool)->jsverify.JSVerify.Property<unit>) (test:'args->bool) = testProperty name test

    let equal expected actual: unit = Expect.equal actual expected ""
    let notEqual expected actual: unit = Expect.notEqual actual expected ""
#endif

open Testing

let throwsError (expected: string) (f: unit -> 'a): unit =
    let success =
        try
            f () |> ignore
            true
        with e ->
            if not <| String.IsNullOrEmpty(expected) then
                equal expected e.Message
            false
    // TODO better error messages
    equal false success