namespace SleepingBearSystem.ArtShowToolsPrototype.Domain

open System

type ArtworkId = ArtworkId of Guid
type ArtworkTitle = ArtworkTitle of string
type ArtworkYear = ArtworkYear of int

type ArtworkCommand =
    | Create of ArtworkCreate
    | ChangeTitle of ArtworkChangeTitle
    | ChangeYear of ArtworkChangeYear

and ArtworkCreate =
    { Id: ArtworkId
      Title: ArtworkTitle
      Year: ArtworkYear }

and ArtworkChangeTitle = { Id: ArtworkId; Title: ArtworkTitle }
and ArtworkChangeYear = { Id: ArtworkId; Year: ArtworkYear }

type ArtworkEvent =
    | Created of ArtworkCreated
    | TitleChanged of ArtworkTitleChanged
    | YearChanged of ArtworkYearChanged

and ArtworkCreated =
    { Id: ArtworkId
      Title: ArtworkTitle
      Year: ArtworkYear }

and ArtworkTitleChanged = { Id: ArtworkId; Title: ArtworkTitle }
and ArtworkYearChanged = { Id: ArtworkId; Year: ArtworkYear }

type ArtworkError =
    | ArtworkAlreadyExists
    | ArtworkDoesNotExist
    | WrongArtwork
    | InvalidArtworkTitle
    | InvalidArtworkYear

type Artwork =
    | Initial
    | Existing of ArtworkInfo

and ArtworkInfo =
    { Id: ArtworkId
      Title: ArtworkTitle
      Year: ArtworkYear }

module Artwork =
    let private applyEvent result event : Result<Artwork, ArtworkError> =
        match result with
        | Error _ -> result
        | Ok artwork ->
            match event with
            | Created created ->
                match artwork with
                | Initial ->
                    Ok(
                        Existing
                            { Id = created.Id
                              Title = created.Title
                              Year = created.Year }
                    )
                | _ -> Error ArtworkAlreadyExists
            | TitleChanged titleChanged ->
                match artwork with
                | Initial -> Error ArtworkDoesNotExist
                | Existing existing ->
                    if (existing.Id = titleChanged.Id) then
                        Ok(Existing { existing with Title = titleChanged.Title })
                    else
                        Error WrongArtwork
            | YearChanged yearChanged ->
                match artwork with
                | Initial -> Error ArtworkDoesNotExist
                | Existing existing ->
                    if (existing.Id = yearChanged.Id) then
                        Ok(Existing { existing with Year = yearChanged.Year })
                    else
                        Error WrongArtwork

    let apply artwork events : Result<Artwork, ArtworkError> =
        events |> List.fold applyEvent (Ok artwork)

    let handle artwork command : Result<ArtworkEvent list, ArtworkError> =
        match command with
        | Create create ->
            match artwork with
            | Initial _ ->
                Ok(
                    Created
                        { Id = create.Id
                          Title = create.Title
                          Year = create.Year }
                    |> List.singleton
                )
            | Existing _ -> Error ArtworkAlreadyExists
        | ChangeTitle changeTitle ->
            match artwork with
            | Initial _ -> Error ArtworkDoesNotExist
            | Existing existing ->
                if (existing.Id = changeTitle.Id) then
                    Ok(
                        TitleChanged
                            { Id = changeTitle.Id
                              Title = changeTitle.Title }
                        |> List.singleton
                    )
                else
                    Error WrongArtwork
        | ChangeYear changeYear ->
            match artwork with
            | Initial -> Error ArtworkDoesNotExist
            | Existing existing ->
                if (existing.Id = changeYear.Id) then
                    Ok(
                        YearChanged
                            { Id = changeYear.Id
                              Year = changeYear.Year }
                        |> List.singleton
                    )
                else
                    Error WrongArtwork

    let applyAndHandle events command : Result<ArtworkEvent list, ArtworkError> =
        events
        |> apply Initial
        |> Result.bind (fun artwork -> command |> handle artwork)
