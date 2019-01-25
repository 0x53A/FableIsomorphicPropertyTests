This test project is "isomorphic", the tests are executed twice: once on .NET, once with JS.

I've copied some of the infrastructure code from https://github.com/fable-compiler/Fable/tree/master/tests/Main



### Isomorphic tests

F# code is supposed to work the same between .NET and JS. But "supposed to" is a big word, and reality doesn't always work that way.

With "Isomorphic Tests" this can actually be validated.

#### .NET

On .NET, the test framework is [Expecto](https://github.com/haf/expecto).
For property tests, it uses [FsCheck](https://github.com/fscheck/FsCheck).


#### JS

On JS, the test framework is [Mocha](https://github.com/mochajs/mocha).
For property tests, it uses [jsverify](https://github.com/jsverify/jsverify).


#### Common Facade

We need a new abstraction layer to abstract these different frameworks away.

There are a few helpers in Util.fs.

### Isomorphic Property tests

For a general introduction to property based testing, see this link: https://fsharpforfunandprofit.com/posts/property-based-testing/.

Defining simple property tests in F# with FsCheck is simple - it just uses Reflection under the hood.

In JS, you need a bit of setup: You need to declare the types of the input parameters.

jsverify Hello World looks something like this:

```typescript
jsc.property("(b && b) === b", jsc.bool, b => (b && b) === b);
```

The first one is just the name. Then you have a few helpers to declare the input parameters which should be generated (jsc.bool, jsc.string, ...). Then the actual test function.

In FsCheck with Expecto, we would write this like

```fsharp
testProperty "(b && b) === b" (fun b -> (b && b) = b)
```

So the core, the test function, is the same, but for JS we need a bit of setup.


There is a new facade: ``testProperty``.

A simple property test could look like this:

```fsharp
testProperty "(b && b) === b" (fun fn -> jsc.forall(jsc.bool, (fun b -> !^ fn(b) ))) (fun b -> (b && b) = b)
```

