module SleepingBearSystems.ArtShowToolsPrototype.Domain.Tests.Testing

open NUnit.Framework

let failOnError<'T, 'E> (message: string) (result: Result<'T, 'E>) : 'T =
    match result with
    | Ok v -> v
    | Error _ -> failwith message

[<Test>]
let ``Testing.failOnError handles Error correctly`` () =
    let ex = Assert.Throws (fun x -> Error "error" |> failOnError "fail" |> ignore)
    Assert.That(ex.Message, Is.EqualTo("fail"))

[<Test>]
let ``Testing.failOnError handles Ok correctly`` () =
    let value = Ok "ok" |> failOnError "fail"
    Assert.That(value, Is.EqualTo("ok"))
