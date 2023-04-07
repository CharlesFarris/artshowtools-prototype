module SleepingBearSystems.ArtShowToolsPrototype.Domain.Tests.ArtworkIdTests

open System
open NUnit.Framework
open SleepingBearSystems.ArtShowToolsPrototype.Domain

let guid = Guid("AD5BB7A9-4AC8-409E-9736-55E0CBB9EED0")

[<Test>]
let ``ArtworkId.fromGuid fails on empty Guid`` () =
    let result = Guid.Empty |> ArtworkId.fromGuid

    match result with
    | Ok _ -> Assert.Fail("method should fail")
    | Error error ->
        match error with
        | InvalidArtworkId -> ()
        | _ -> Assert.Fail("unexpected error type")

[<Test>]
let ``ArtworkId.fromGuid succeeds`` () =
    let result = guid |> ArtworkId.fromGuid

    match result with
    | Ok artworkId -> Assert.That(artworkId |> ArtworkId.toGuid, Is.EqualTo(guid))
    | Error _ -> Assert.Fail("method should succeed")
