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
        | _ -> Ok List.empty
