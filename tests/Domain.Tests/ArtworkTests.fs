module SleepingBearSystem.ArtShowToolsPrototype.Domain.Tests

open System
open NUnit.Framework

let id = Guid("350EA2A6-6316-44DE-9316-2D545E5CA2C5") |> ArtworkId
let title = "Title" |> ArtworkTitle
let year = 2023 |> ArtworkYear
let artwork = Existing { Id = id; Title = title; Year = year }
let wrongId = Guid("D96CD8A5-E757-4798-8753-769E710AFA5A") |> ArtworkId

let wrongArtwork =
    Existing
        { Id = wrongId
          Title = title
          Year = year }

let newTitle = "New Title" |> ArtworkTitle
let newYear = 2025 |> ArtworkYear

[<Test>]
let ``Artwork.apply Created succeeds`` () =
    let events = Created { Id = id; Title = title; Year = year } |> List.singleton
    let result = events |> Artwork.apply Initial

    match result with
    | Ok updated ->
        match updated with
        | Initial _ -> Assert.Fail("unexpected state")
        | Existing existing ->
            Assert.That(existing.Id, Is.EqualTo(id))
            Assert.That(existing.Title, Is.EqualTo(title))
            Assert.That(existing.Year, Is.EqualTo(year))
    | Error _ -> Assert.Fail("apply should succeed")

[<Test>]
let ``Artwork.apply Created fails if artwork exists`` () =
    let events = Created { Id = id; Title = title; Year = year } |> List.singleton
    let result = events |> Artwork.apply artwork

    match result with
    | Ok _ -> Assert.Fail("apply should not succeed")
    | Error error ->
        match error with
        | ArtworkAlreadyExists -> ()
        | _ -> Assert.Fail("unexpected error type")

[<Test>]
let ``Artwork.apply TitleChanged succeeds`` () =
    let events = TitleChanged { Id = id; Title = newTitle } |> List.singleton
    let result = events |> Artwork.apply artwork

    match result with
    | Ok updated ->
        match updated with
        | Initial _ -> Assert.Fail("unexpected state")
        | Existing existing ->
            Assert.That(existing.Id, Is.EqualTo(id))
            Assert.That(existing.Title, Is.EqualTo(newTitle))
    | Error _ -> Assert.Fail("apply should have succeeded")

[<Test>]
let ``Artwork.apply TitleChanged fails if artwork does not exist`` () =
    let events = TitleChanged { Id = id; Title = newTitle } |> List.singleton
    let result = events |> Artwork.apply Initial

    match result with
    | Ok _ -> Assert.Fail("apply should not have succeeded")
    | Error error ->
        match error with
        | ArtworkDoesNotExist -> ()
        | _ -> Assert.Fail("unexpected error type")

[<Test>]
let ``Artwork.apply TitleChanged fails on wrong ID`` () =
    let events = TitleChanged { Id = id; Title = newTitle } |> List.singleton
    let result = events |> Artwork.apply wrongArtwork

    match result with
    | Ok _ -> Assert.Fail("apply should not have succeeded")
    | Error error ->
        match error with
        | WrongArtwork -> ()
        | _ -> Assert.Fail("unexpected error type")


[<Test>]
let ``Artwork.apply YearChanged succeeds`` () =
    let events = YearChanged { Id = id; Year = newYear } |> List.singleton
    let result = events |> Artwork.apply artwork

    match result with
    | Ok updated ->
        match updated with
        | Initial -> Assert.Fail("unexpected state")
        | Existing existing ->
            Assert.That(existing.Id, Is.EqualTo(id))
            Assert.That(existing.Year, Is.EqualTo(newYear))
    | Error _ -> Assert.Fail("apply should have succeeded")

[<Test>]
let ``Artwork.apply YearChanged fails if artwork does not exist`` () =
    let events = YearChanged { Id = id; Year = newYear } |> List.singleton
    let result = events |> Artwork.apply Initial

    match result with
    | Ok _ -> Assert.Fail("apply should not have succeeded")
    | Error error ->
        match error with
        | ArtworkDoesNotExist -> ()
        | _ -> Assert.Fail("unexpected error type")

[<Test>]
let ``Artwork.apply YearChanged fails on wrong ID`` () =
    let events = YearChanged { Id = id; Year = newYear } |> List.singleton
    let result = events |> Artwork.apply wrongArtwork

    match result with
    | Ok _ -> Assert.Fail("apply should not have succeeded")
    | Error error ->
        match error with
        | WrongArtwork -> ()
        | _ -> Assert.Fail("unexpected error type")

[<Test>]
let ``Artwork.handle Create fails if artwork exists`` () =
    let command = Create { Id = id; Title = title; Year = year }
    let result = command |> Artwork.handle artwork

    match result with
    | Ok _ -> Assert.Fail("command should fail")
    | Error error ->
        match error with
        | ArtworkAlreadyExists -> ()
        | _ -> Assert.Fail("unexpected error type")

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

[<Test>]
let ``Artwork.handle ChangeTitle fails if artwork does not exist`` () =
    let command = ChangeTitle { Id = id; Title = newTitle }
    let result = command |> Artwork.handle Initial

    match result with
    | Ok _ -> Assert.Fail("command should not succeed")
    | Error error ->
        match error with
        | ArtworkDoesNotExist -> ()
        | _ -> Assert.Fail("unexpected error type")

[<Test>]
let ``Artwork.handle ChangeTitle fails on wrong ID`` () =
    let command = ChangeTitle { Id = id; Title = newTitle }
    let result = command |> Artwork.handle wrongArtwork

    match result with
    | Ok _ -> Assert.Fail("command should not succeed")
    | Error error ->
        match error with
        | WrongArtwork -> ()
        | _ -> Assert.Fail("unexpected error type")

[<Test>]
let ``Artwork.handle ChangeTitle succeeds`` () =
    let command = ChangeTitle { Id = id; Title = newTitle }
    let result = command |> Artwork.handle artwork

    match result with
    | Ok events ->
        Assert.That(events.Length, Is.EqualTo(1))

        match events |> List.head with
        | TitleChanged titleChanged ->
            Assert.That(titleChanged.Id, Is.EqualTo(id))
            Assert.That(titleChanged.Title, Is.EqualTo(newTitle))
        | _ -> Assert.Fail("unexpected event type")
    | Error _ -> Assert.Fail("command should succeed")

[<Test>]
let ``Artwork.handle ChangeYear fails if artwork does not exist`` () =
    let command = ChangeYear { Id = id; Year = newYear }
    let result = command |> Artwork.handle Initial

    match result with
    | Ok _ -> Assert.Fail("command should not succeed")
    | Error error ->
        match error with
        | ArtworkDoesNotExist -> ()
        | _ -> Assert.Fail("unexpected error type")

[<Test>]
let ``Artwork.handle ChangeYear fails on wrong ID`` () =
    let command = ChangeYear { Id = id; Year = newYear }
    let result = command |> Artwork.handle wrongArtwork

    match result with
    | Ok _ -> Assert.Fail("command should not succeed")
    | Error error ->
        match error with
        | WrongArtwork -> ()
        | _ -> Assert.Fail("unexpected error type")

[<Test>]
let ``Artwork.handle ChangeYear succeeds`` () =
    let command = ChangeYear { Id = id; Year = newYear }
    let result = command |> Artwork.handle artwork

    match result with
    | Ok events ->
        Assert.That(events.Length, Is.EqualTo(1))

        match events |> List.head with
        | YearChanged yearChanged ->
            Assert.That(yearChanged.Id, Is.EqualTo(id))
            Assert.That(yearChanged.Year, Is.EqualTo(newYear))
        | _ -> Assert.Fail("unexpected event type")
    | Error _ -> Assert.Fail("command should not succeed")

