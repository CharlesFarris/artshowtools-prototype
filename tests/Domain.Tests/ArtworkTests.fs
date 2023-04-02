module SleepingBearSystem.ArtShowToolsPrototype.Domain.Tests

open System
open NUnit.Framework

let id = Guid("350EA2A6-6316-44DE-9316-2D545E5CA2C5") |> ArtworkId
let title = "Title" |> ArtworkTitle
let year = 2023 |> ArtworkYear
let artwork = Existing { Id = id; Title = title; Year = year }

[<Test>]
let ``Artwork.handle Create fails if artwork exists`` () =
    let command = Create { Id = id; Title = title; Year = year }
    let result = command |> Artwork.handle artwork
    match result with
    | Ok _ -> Assert.Fail("command should fail")
    | Error error ->
        match error with
        | ArtworkAlreadyExists -> ()
        | _ -> Assert.Fail("wrong error type")

[<Test>]
let ``Artwork.handle Create succeeds`` () =
    let command = Create { Id = id; Title = title; Year = year }
    let result = command |> Artwork.handle Initial
    match result with
    | Ok events ->
        Assert.That(events.Length, Is.EqualTo(1))
        match events |> List.head with
        | Created created ->
            Assert.That(created.Id, Is.EqualTo(id))
            Assert.That(created.Title, Is.EqualTo(title))
            Assert.That(created.Year, Is.EqualTo(year))
        | _ -> Assert.Fail("unexpected event type")
    | Error _ -> Assert.Fail("command should succeed")
