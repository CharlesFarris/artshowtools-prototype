module SleepingBearSystems.ArtShowToolsPrototype.Domain.Tests.ArtworkYearTests

open NUnit.Framework
open SleepingBearSystems.ArtShowToolsPrototype.Domain

[<Test>]
let ``ArtworkYear.fromInteger fails on value less than 1`` () =
    let result = 0 |> ArtworkYear.fromInteger

    match result with
    | Ok _ -> Assert.Fail("method should fail")
    | Error error ->
        match error with
        | InvalidArtworkYear -> ()
        | _ -> Assert.Fail("unexpected error type")

[<Test>]
let ``ArtworkYear.fromInteger succeeds`` () =
    let result = 2023 |> ArtworkYear.fromInteger

    match result with
    | Ok artworkId -> Assert.That(artworkId |> ArtworkYear.toInteger, Is.EqualTo(2023))
    | Error _ -> Assert.Fail("method should succeed")
