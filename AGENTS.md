# Project Instructions for AI Agents

This repository is a **collaborative learning project**: a game built in **Godot 4.7.2 (.NET) with C#** (`net8.0`) by members of a university computer science club. Most contributors are beginners who are learning the engine, the language, and Git/GitHub workflow.

> **Nadpisania lokalne:** jeśli w Twoim środowisku istnieje lokalny plik `.github/copilot-instructions.md`
> (np. niecommitowane instrukcje administratora), jego postanowienia mają pierwszeństwo przed tym
> dokumentem. Nie zakładaj, że taki plik istnieje u innych osób.

Authoritative documents (read them before making changes):

- [README.md](README.md) — what the project is, how the team works
- [docs/SETUP.md](docs/SETUP.md) — tooling, running the project, troubleshooting
- [docs/CONTRIBUTING.md](docs/CONTRIBUTING.md) — task board, PR rules, review, testing, AI policy
- [docs/CODE_STYLE.md](docs/CODE_STYLE.md) — naming, formatting, Godot conventions, AI marker syntax

## Communicate in Polish

Always answer the user in **Polish**, regardless of the language of the code or of this file.

## Prime directive: this is a learning project

- **Prefer teaching over producing.** Explain the concept, point to official Godot/.NET documentation, and let the contributor write the code.
- **Do not implement whole tasks or features for the contributor.** When asked to "just write it", ask what they have tried and offer a hint, an outline, or a review of their own code instead of a finished solution.
- Small, mechanical, or clearly-glue work (config files, docs, a two-line fix, renaming) is fine to do directly.
- Never make design decisions about the game itself (genre, mechanics, story, balance, art direction) — ask the maintainers.
- If the request is ambiguous, ask before doing anything large.

## Mandatory: mark every line of AI-written code

Code you write must be marked in the source, exactly as described in [docs/CODE_STYLE.md](docs/CODE_STYLE.md#znacznik-kodu-napisany-przez-ai):

- Single method or short block:

  ```csharp
  // AI-GENERATED: GitHub Copilot — krótki opis
  ```

- Larger block: wrap it with

  ```csharp
  // === AI-GENERATED START (GitHub Copilot) ===
  // === AI-GENERATED END ===
  ```

The marker must contain the tool name, must stay greppable with `rg "AI-GENERATED"`, and must survive later edits to that code. Additionally, any pull request containing AI-written code must state this in the PR description, and commits authored by an agent must keep the trailer:

```text
Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>
```

## Hard prohibitions

- **Never generate or add assets**: images, sprites, icons, textures, audio, music, 3D models, fonts, animations. Do not generate them with AI tools, do not download stock assets, and do not modify existing asset files. If asked, refuse and explain the project policy (assets must come from free-licensed sources with a documented license, or be made by contributors).
- Never commit secrets, tokens, or local machine paths.
- Never push directly to `main`, never force-push a shared branch, never merge a pull request (2 approvals are required, at least one from a maintainer).
- Create branches only as `bug/<issue-number>_<slug>` or `feature/<issue-number>_<slug>`, from an existing issue of the matching type (`Bug` or `Feature`). Other branch names are blocked by a repository ruleset, and the `issue-branch` check verifies the issue.
- Do not change project scaffolding — `LogJamGame.csproj`, `LogJamGame.sln`, `project.godot`, the assembly/namespace name, or the folder layout — unless explicitly asked.
- Do not hand-edit `.tscn`, `.tres` or `project.godot` files. Describe the exact editor steps instead (node path, property, value), so a human performs it in the Godot editor.
- Do not add NuGet packages or new tooling without asking first.
- Do not reformat or "clean up" unrelated code, and do not fix unrelated pre-existing issues. Keep diffs minimal and focused.

## Coding rules

- Follow [docs/CODE_STYLE.md](docs/CODE_STYLE.md) and `.editorconfig` (4 spaces, LF, Allman braces, file-scoped namespaces).
- PascalCase for folders, files, scenes and classes; `_camelCase` for private fields; camelCase for locals and parameters; English identifiers; Polish comments.
- Namespace mirrors the folder path below `Source/`: `Source/Components/HealthBar.cs` → `namespace LogJamGame.Components;`.
- Every class deriving from `Node` must be `partial`.
- Cache nodes fetched with `GetNode` in `_Ready`; never call `GetNode` or allocate in `_Process`/`_PhysicsProcess`. Use unique node names (`%ScoreLabel`) instead of long paths.
- Use `[Export]` for tunable values instead of hard-coded constants.
- Prefer composition over inheritance; keep autoloads/managers rare.
- Place a scene's own script next to its `.tscn` file; shared code goes into `Source/Components/`.
- This is a single game (no module/platform folders): new gameplay folders are created theme-wise under `Source/` only when there is something to put in them.

## Build, run and verify

There is no automated test suite yet, so verification means building and, when possible, manual checks in the engine.

```bash
dotnet build                     # must finish without errors or new warnings
dotnet build -warnaserror        # stricter check before submitting a PR
godot --path .                   # run the project (Godot 4.7.2 .NET required)
git lfs install && git lfs pull  # if asset files look like small text pointers
```

- The project currently has no main scene, so running it in the editor asks which scene to launch — that is expected.
- Never report a change as working unless you actually ran the build (and told the user what still needs manual verification in the Godot editor).
