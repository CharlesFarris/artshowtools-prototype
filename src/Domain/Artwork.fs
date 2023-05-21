namespace SleepingBearSystems.ArtShowToolsPrototype.Domain

open System

type ArtworkId = private ArtworkId of Guid
type ArtworkTitle = private ArtworkTitle of string
type ArtworkYear = private ArtworkYear of int

type ArtworkCommand =
    | Create of ArtworkCreate
    | ChangeMetadata of ArtworkChangeMetadata

and ArtworkCreate =
    { Id: ArtworkId
      Title: ArtworkTitle
      Year: ArtworkYear }

and ArtworkChangeMetadata =
    { Id: ArtworkId
      Title: ArtworkTitle
      Year: ArtworkYear }

type ArtworkEvent =
    | Created of ArtworkCreated
    | MetadataChanged of ArtworkMetadataChanged

and ArtworkCreated =
    { Id: ArtworkId
      Title: ArtworkTitle
      Year: ArtworkYear }

and ArtworkMetadataChanged =
    { Id: ArtworkId
      Title: ArtworkTitle
      Year: ArtworkYear }

type ArtworkError =
    | ArtworkAlreadyExists
    | ArtworkDoesNotExist
    | WrongArtwork
    | InvalidArtworkTitle
    | InvalidArtworkYear
    | InvalidArtworkId

type Artwork =
    | Initial
    | Existing of ArtworkInfo

and ArtworkInfo =
    { Id: ArtworkId
      Title: ArtworkTitle
      Year: ArtworkYear }

module ArtworkId =
    let fromGuid (id: Guid) : Result<ArtworkId, ArtworkError> =
        if id = Guid.Empty then
            Error InvalidArtworkId
        else
            Ok(id |> ArtworkId)

    let toGuid (artworkId: ArtworkId) : Guid =
        let (ArtworkId id) = artworkId
        id

module ArtworkTitle =
    let fromString (title: string) : Result<ArtworkTitle, ArtworkError> =
        if String.IsNullOrWhiteSpace(title) then
            Error InvalidArtworkTitle
        else if title.Length > 64 then
            Error InvalidArtworkTitle
        else
            Ok(title |> ArtworkTitle)

    let toString (artworkTitle: ArtworkTitle) : string =
        let (ArtworkTitle title) = artworkTitle
        title

module ArtworkYear =
    let fromInteger (year: int) : Result<ArtworkYear, ArtworkError> =
        if year < 1 then
            Error InvalidArtworkYear
        else
            Ok(year |> ArtworkYear)

    let toInteger (artworkYear: ArtworkYear) : int =
        let (ArtworkYear year) = artworkYear
        year

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
            | MetadataChanged metadataChanged ->
                match artwork with
                | Initial -> Error ArtworkDoesNotExist
                | Existing existing ->
                    if (existing.Id = metadataChanged.Id) then
                        Ok(
                            Existing
                                { existing with
                                    Title = metadataChanged.Title
                                    Year = metadataChanged.Year }
                        )
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
        | ChangeMetadata changeMetadata ->
            match artwork with
            | Initial _ -> Error ArtworkDoesNotExist
            | Existing existing ->
                if (existing.Id = changeMetadata.Id) then
                    Ok(
                        MetadataChanged
                            { Id = changeMetadata.Id
                              Title = changeMetadata.Title
                              Year = changeMetadata.Year }
                        |> List.singleton
                    )
                else
                    Error WrongArtwork

    let applyAndHandle events command : Result<ArtworkEvent list, ArtworkError> =
        events
        |> apply Initial
        |> Result.bind (fun artwork -> command |> handle artwork)
