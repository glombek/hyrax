# Hyrax Agents

## Overview
Hyrax is an ActivityPub implementation for Umbraco CMS that enables federated publishing and social networking capabilities.

## Core Components

### Controllers
- **ActivityPubController**: Implements ActivityPub protocol endpoints (Actor, Outbox, Inbox)
- **WebFingerController**: Enables actor discovery via WebFinger protocol
- **RssController**: RSS feed generation

### Services
- **IHyraxAuthorService**: Manages author/actor data
- **IHyraxResourceLocatorService**: Locates and retrieves published resources
- **IHyraxSignatureRepositoryService**: Manages cryptographic signatures for federation
- **IHyraxActivityService**: Handles ActivityPub activity processing and storage

### Models
- ActivityPub domain models: Actor, CreateActivity, NoteObject, OrderedCollectionPage, PublicKey, Hashtag
- Core domain models: Author, Resource, Publication

## Key Features
- ActivityPub compliant federation with Mastodon and other fediverse platforms
- Actor profile exposure with public key for HTTP signature verification
- Paginated activity streams (Outbox, Inbox)
- WebFinger discovery protocol
- RSS feed generation alongside ActivityPub endpoints

## Target
- .NET 10
- Umbraco CMS integration
