#!/usr/bin/env bash
#
# Generuje treść wydania (Markdown) na podstawie commitów z historii Git.
# Użycie: generate-changelog.sh <poprzedni-tag> <wersja-bez-v> <tag>
# Przykład: generate-changelog.sh v0.1.0 0.1.1 v0.1.1
# Pusty poprzedni tag oznacza pierwsze wydanie.

set -euo pipefail

previous="${1:-}"
version="${2:?Brak numeru wersji}"
tag="${3:-v$version}"
repo="${GITHUB_REPOSITORY:-Kolo-Naukowe-Informatykow/LogJamGame}"
server="${GITHUB_SERVER_URL:-https://github.com}"

if [ -n "$previous" ]; then
    range="${previous}..HEAD"
    comparison="${server}/${repo}/compare/${previous}...${tag}"
    heading="Zmiany od wydania \`${previous}\`:"
else
    range="HEAD"
    comparison="${server}/${repo}/commits/${tag}"
    heading="Pierwsze wydanie projektu:"
fi

commit_count="$(git rev-list --count --no-merges "$range")"

echo "# LogJamGame ${tag}"
echo
echo "$heading"
echo
echo "**Commity:** ${commit_count} · **Pełna lista zmian:** ${comparison}"
echo

if [ "$commit_count" -eq 0 ]; then
    echo "_Brak nowych commitów._"
    echo
else
    git log "$range" --no-merges --reverse \
        --pretty=format:'- %s ([%h]('"${server}/${repo}/commit/%H"'))'
    echo
fi

echo
echo "## Pliki do pobrania"
echo
echo "- \`LogJamGame-${version}-windows-x86_64.zip\` — Windows (x86_64)"
echo "- \`LogJamGame-${version}-linux-x86_64.zip\` — Linux (x86_64)"
echo "- \`LogJamGame-${version}-macos-universal.zip\` — macOS (Universal 2, bez podpisu)"
echo

echo "## Uwagi"
echo

if ! grep -q '^run/main_scene=' project.godot; then
    echo "- W tym wydaniu nie ma jeszcze sceny głównej — build jest kompletny, ale gra nie uruchomi się poprawnie."
fi

echo "- Buildy są **niepodpisane**. macOS może wymagać obejścia Gatekeepera (prawy przycisk → *Open*)."
echo "- W każdym archiwum znajduje się \`CREDITS.md\` z listą użytych assetów i ich licencji."
echo "- Numer wersji pochodzi z tagów Git; wydanie tworzy workflow [\`release.yml\`](${server}/${repo}/blob/main/.github/workflows/release.yml)."
