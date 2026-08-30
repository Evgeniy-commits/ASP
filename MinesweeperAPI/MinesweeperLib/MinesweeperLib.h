#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace MinesweeperProxyLib
{
    public enum class CellState
    {
        Hidden,
        Revealed,
        Flagged
    };

    public ref class GameModel
    {
    private:
        int Rows;
        int Cols;
        int MineCount;

        array<bool>^ _isMine;
        array<int>^ _neighborCount;
        array<CellState>^ _state;

        bool IsGameOver;
        bool IsWon;

        Random^ _random;

        int GetIndex(int r, int c);
        void Initialize();
        void RevealEmptyArea(int startR, int startC);
        void CheckWin();

    public:
        property int RowsProperty { int get() { return Rows; } }
        property int ColsProperty { int get() { return Cols; } }
        property int MineCountProperty { int get() { return MineCount; } }
        property bool IsGameOverProperty { bool get() { return IsGameOver; } }
        property bool IsWonProperty { bool get() { return IsWon; } }
        property int TotalMines {int get() { return MineCount; } }

        GameModel(int rows, int cols, int mineCount);

        CellState GetState(int r, int c);
        bool IsMine(int r, int c);
        int GetNeighborCount(int r, int c);

        // Основной метод открытия клетки (ЛКМ)
        void Reveal(int r, int c);

        // Переключение флага (ПКМ)
        void ToggleFlag(int r, int c);

        // Chording: открытие соседей, если число вокруг равно количеству флагов
        void Chord(int r, int c);

        void Restart();
    };
}