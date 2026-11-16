namespace LogJamGame.Tmp;

/// <summary>
/// Warstwy kolizji prototypu. Gracz i przeciwnicy stoją na osobnych warstwach, żeby
/// nie zderzali się ze sobą ani między sobą — dzięki temu przeciwnik melee nie „przykleja się”
/// do gracza i może przez niego przepłynąć. Wszyscy nadal kolidują ze ścianami areny,
/// a pociski wykrywają wszystkie trzy warstwy.
/// </summary>
public static class CollisionLayers
{
    public const uint World = 1;
    public const uint Player = 2;
    public const uint Enemy = 4;
}
