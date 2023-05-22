module SleepingBearSystems.ArtShowTools.Prototype.Domain.Tests.ArtworkTitleTests

open System
open NUnit.Framework
open SleepingBearSystems.ArtShowTools.Prototype.Domain

[<Test>]
let ``ArtworkTitle.fromString fails on empty string`` () =
    let result = "" |> ArtworkTitle.fromString

    match result with
    | Ok _ -> Assert.Fail("method should fail")
    | Error error ->
        match error with
        | InvalidArtworkTitle -> ()
        | _ -> Assert.Fail("unexpected error type")

[<Test>]
let ``ArtworkTitle.fromString fails if string is longer than 64`` () =
    let result = String('a', 65) |> ArtworkTitle.fromString

    match result with
    | Ok _ -> Assert.Fail("method should fail")
    | Error error ->
        match error with
        | InvalidArtworkTitle -> ()
        | _ -> Assert.Fail("unexpected error type")

[<Test>]
let ``ArtworkTitle.fromString succeeds`` () =
    let result = "title" |> ArtworkTitle.fromString

    match result with
    | Ok artworkId -> Assert.That(artworkId |> ArtworkTitle.toString, Is.EqualTo("title"))
    | Error _ -> Assert.Fail("method should succeed")
