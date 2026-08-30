#include "pch.h"
#include "MinesweeperLib.h"

using namespace MinesweeperProxyLib;
using namespace System;
using namespace System::Diagnostics;

int GameModel::GetIndex(int r, int c)
{
    return r * Cols + c;
}

GameModel::GameModel(int rows, int cols, int mineCount)
{
    if (rows <= 0 || cols <= 0 || mineCount < 0 || mineCount >= rows * cols)
        throw gcnew ArgumentException("Invalid game parameters");

    Rows = rows;
    Cols = cols;
    MineCount = mineCount;

    int totalCells = rows * cols;
    _isMine = gcnew array<bool>(totalCells);
    _neighborCount = gcnew array<int>(totalCells);
    _state = gcnew array<CellState>(totalCells);

    // Инициализируем массивы
    for (int i = 0; i < totalCells; i++)
    {
        _isMine[i] = false;
        _neighborCount[i] = 0;
        _state[i] = CellState::Hidden;
    }

    _random = gcnew Random(DateTime::Now.Ticks);

    Initialize();
}

void GameModel::Initialize()
{
    int totalCells = Rows * Cols;

    // Сброс состояний (на случай рестарта)
    for (int i = 0; i < totalCells; i++)
    {
        _isMine[i] = false;
        _neighborCount[i] = 0;
        _state[i] = CellState::Hidden;
    }

    // Расстановка мин
    int placed = 0;
    while (placed < MineCount)
    {
        int r = _random->Next(Rows);
        int c = _random->Next(Cols);
        int idx = GetIndex(r, c);

        if (!_isMine[idx])
        {
            _isMine[idx] = true;
            placed++;
        }
    }

    // Подсчёт соседей (чисел)
    for (int r = 0; r < Rows; ++r)
    {
        for (int c = 0; c < Cols; ++c)
        {
            int idx = GetIndex(r, c);
            if (_isMine[idx]) continue; // Сама мина не хранит число

            int count = 0;
            for (int dr = -1; dr <= 1; ++dr)
            {
                for (int dc = -1; dc <= 1; ++dc)
                {
                    if (dr == 0 && dc == 0) continue;

                    int nr = r + dr;
                    int nc = c + dc;

                    if (nr >= 0 && nr < Rows && nc >= 0 && nc < Cols)
                    {
                        if (_isMine[GetIndex(nr, nc)])
                            count++;
                    }
                }
            }
            _neighborCount[idx] = count;
        }
    }

    IsGameOver = false;
    IsWon = false;
}

CellState GameModel::GetState(int r, int c)
{
    if (r < 0 || r >= Rows || c < 0 || c >= Cols)
        throw gcnew ArgumentOutOfRangeException("Coordinates out of range");
    return _state[GetIndex(r, c)];
}

bool GameModel::IsMine(int r, int c)
{
    if (r < 0 || r >= Rows || c < 0 || c >= Cols)
        return false;
    return _isMine[GetIndex(r, c)];
}

int GameModel::GetNeighborCount(int r, int c)
{
    if (r < 0 || r >= Rows || c < 0 || c >= Cols)
        return 0;
    return _neighborCount[GetIndex(r, c)];
}

// Приватный статический метод (или можно сделать private в классе)
static int CountFlaggedNeighbors(array<CellState>^ state, int r, int c, int rows, int cols)
{
    int count = 0;
    for (int dr = -1; dr <= 1; dr++)
    {
        for (int dc = -1; dc <= 1; dc++)
        {
            if (dr == 0 && dc == 0) continue;

            int nr = r + dr;
            int nc = c + dc;

            if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
            {
                if (state[nr * cols + nc] == CellState::Flagged)
                    count++;
            }
        }
    }
    return count;
}

void GameModel::Reveal(int r, int c)
{
    if (IsGameOver) return;
    if (r < 0 || r >= Rows || c < 0 || c >= Cols) return;

    int idx = GetIndex(r, c);

    // Не открываем уже открытые или помеченные флагом клетки
    if (_state[idx] != CellState::Hidden)
        return;

    // Если попали на мину — проигрыш
    if (_isMine[idx])
    {
        IsGameOver = true;
        return;
    }

    // Сначала помечаем как открытую
    _state[idx] = CellState::Revealed;

    // Если вокруг нет мин — запускаем автооткрытие пустой области
    if (_neighborCount[idx] == 0)
    {
        RevealEmptyArea(r, c);
    }

    CheckWin();
}

void GameModel::Chord(int r, int c)
{
    // Логика «одновременного нажатия» (chord)
    if (IsGameOver) return;
    if (r < 0 || r >= Rows || c < 0 || c >= Cols) return;

    int idx = GetIndex(r, c);
    if (_state[idx] != CellState::Revealed)
        return; // Chord работает только на открытых клетках

    int neighborCount = _neighborCount[idx];
    int flaggedNeighbors = CountFlaggedNeighbors(_state, r, c, Rows, Cols);

    if (neighborCount != flaggedNeighbors)
        return; // Недостаточно флагов или их слишком много

    // Открываем всех скрытых соседей
    for (int dr = -1; dr <= 1; dr++)
    {
        for (int dc = -1; dc <= 1; dc++)
        {
            if (dr == 0 && dc == 0) continue;

            int nr = r + dr;
            int nc = c + dc;

            if (nr >= 0 && nr < Rows && nc >= 0 && nc < Cols)
            {
                int nIdx = GetIndex(nr, nc);
                if (_state[nIdx] == CellState::Hidden)
                {
                    // Если сосед — мина, то проигрыш
                    if (_isMine[nIdx])
                    {
                        IsGameOver = true;
                        return;
                    }

                    _state[nIdx] = CellState::Revealed;
                    if (_neighborCount[nIdx] == 0)
                    {
                        RevealEmptyArea(nr, nc);
                    }
                }
            }
        }
    }

    CheckWin();
}

void GameModel::RevealEmptyArea(int startR, int startC)
{
    auto queue = gcnew Queue<Tuple<int, int>^>();

    int startIdx = GetIndex(startR, startC);
    // Стартовую клетку мы уже пометили как Revealed в вызывающем коде,
    // но если вдруг нет — помечаем здесь:
    if (_state[startIdx] == CellState::Hidden)
    {
        _state[startIdx] = CellState::Revealed;
    }

    queue->Enqueue(Tuple::Create(startR, startC));

    while (queue->Count > 0)
    {
        auto current = queue->Dequeue();
        int r = current->Item1;
        int c = current->Item2;
        int idx = GetIndex(r, c);

        // Если у текущей клетки есть мины вокруг — она не запускает автораскрытие дальше,
        // но её соседи всё равно должны быть раскрыты (если они скрыты).
        // Поэтому мы НЕ делаем continue здесь.

        for (int dr = -1; dr <= 1; ++dr)
        {
            for (int dc = -1; dc <= 1; ++dc)
            {
                if (dr == 0 && dc == 0) continue;

                int nr = r + dr;
                int nc = c + dc;

                if (nr < 0 || nr >= Rows || nc < 0 || nc >= Cols)
                    continue;

                int nIdx = GetIndex(nr, nc);

                // Раскрываем только скрытые клетки
                if (_state[nIdx] == CellState::Hidden)
                {
                    _state[nIdx] = CellState::Revealed;  // сразу помечаем

                    // Добавляем в очередь только если вокруг нет мин —
                    // именно такие клетки продолжат автораскрытие
                    if (_neighborCount[nIdx] == 0)
                    {
                        queue->Enqueue(Tuple::Create(nr, nc));
                    }
                }
            }
        }
    }
}

void GameModel::ToggleFlag(int r, int c)
{
    if (IsGameOver) return;
    if (r < 0 || r >= Rows || c < 0 || c >= Cols) return;

    int idx = GetIndex(r, c);
    if (_state[idx] == CellState::Revealed) return;

    if (_state[idx] == CellState::Flagged)
    {
        _state[idx] = CellState::Hidden;
    }
    else if (_state[idx] == CellState::Hidden)
    {
        _state[idx] = CellState::Flagged;
    }

    CheckWin();
}

void GameModel::CheckWin()
{
    if (IsGameOver) return;

    int revealedNonMines = 0;
    int totalNonMines = Rows * Cols - MineCount;

    for (int r = 0; r < Rows; r++)
    {
        for (int c = 0; c < Cols; c++)
        {
            int idx = GetIndex(r, c);
            if (_state[idx] == CellState::Revealed && !_isMine[idx])
            {
                revealedNonMines++;
            }
        }
    }

    if (revealedNonMines == totalNonMines)
    {
        IsWon = true;
        IsGameOver = true;
    }
}

void GameModel::Restart()
{
    Initialize();
}