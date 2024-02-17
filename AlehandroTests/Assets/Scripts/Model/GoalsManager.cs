using System;

public class GoalsManager
{
    public event Action OnVictoryAchived;
    public event Action OnGameOver;
    public event Action<int> OnScoreValueChanged;
    public event Action<int> OnAmaountOfTokensToDestroyValueChanged;
    public event Action<int> OnAmountOfObstaclesLeftValueChanged;
    public event Action<int> OnMovesLeftValueChanged;

    public int _tokenToDestroy { get; private set; }
    public int _amountOfTokensToDestroy { get; private set; }
    public int _obstacleToDestroy { get; private set; }
    public int _amountOfObstaclesToDestroy { get; private set; }
    public int _targetScore { get; private set; }
    public int _movesLeft { get; private set; }
    private int _scoreValue;
    private bool _won;

    public GoalsManager()
    {
        _tokenToDestroy = -1;
        _obstacleToDestroy = -1;
        _won = false;
    }

    public GoalsManager(int tokenToDestroy, int amountOfTokensToDestroy, int obstacleToDestroy, int amountOfObstaclesToDestroy, int targetScore, int movesLeft)
    {
        Initialize(tokenToDestroy, amountOfTokensToDestroy, obstacleToDestroy, amountOfObstaclesToDestroy, targetScore, movesLeft);
    }

    public GoalsManager(LevelData levelData)
    {
        Initialize(levelData);
    }

    public void Initialize(int tokenToDestroy, int amountOfTokensToDestroy, int obstacleToDestroy, int amountOfObstaclesToDestroy, int targetScore, int movesLeft)
    {
        _tokenToDestroy = tokenToDestroy;
        _amountOfTokensToDestroy = amountOfTokensToDestroy;
        _obstacleToDestroy = obstacleToDestroy;
        _amountOfObstaclesToDestroy = amountOfObstaclesToDestroy;
        _targetScore = targetScore;
        _movesLeft = movesLeft;
        _scoreValue = 0;
        _won = false;
    }

    public void Initialize(LevelData levelData)
    {
        Initialize(levelData.TokenToDestroy, levelData.AmountOfTokensToDestroy, levelData.ObstacleToDestroy, levelData.AmountOfObstaclesToDestroy, levelData.ScoreToGet, levelData.MovesLeft);
    }

    public void AddScore(int value)
    {
        _scoreValue += value;
        OnScoreValueChanged?.Invoke(_scoreValue);

        if (_won) return;
        if (WinConditionAchived()) OnVictoryAchived?.Invoke();
    }

    public void SubtructAmountOfTokens(int value)
    {
        _amountOfTokensToDestroy -= value;
        if (_amountOfTokensToDestroy < 0) _amountOfTokensToDestroy = 0;
        OnAmaountOfTokensToDestroyValueChanged?.Invoke(_amountOfTokensToDestroy);

        if (_won) return;
        if (WinConditionAchived()) OnVictoryAchived?.Invoke();
    }

    public void SubtructAmountOfObstacles(int value)
    {
        _amountOfObstaclesToDestroy -= value;
        if (_amountOfObstaclesToDestroy < 0) _amountOfObstaclesToDestroy = 0;
        OnAmountOfObstaclesLeftValueChanged?.Invoke(_amountOfObstaclesToDestroy);

        if (_won) return;
        if (WinConditionAchived()) OnVictoryAchived?.Invoke();
    }

    public void SubtructAmountOfMoves(int value)
    {
        _movesLeft -= value;
        if (_movesLeft < 0) _movesLeft = 0;
        OnMovesLeftValueChanged?.Invoke(_movesLeft);

        if (_movesLeft < 1) OnGameOver?.Invoke();
    }

    private bool WinConditionAchived()
    {
        if ((_scoreValue >= _targetScore) &&
            (((_amountOfTokensToDestroy < 1) && (_tokenToDestroy > -1)) || (_tokenToDestroy == -1)) &&
            (((_amountOfObstaclesToDestroy < 1) && (_obstacleToDestroy > -1)) || (_obstacleToDestroy == -1)))
        {
            _won = true;
            return true; 
        }

        return false;
    }
}