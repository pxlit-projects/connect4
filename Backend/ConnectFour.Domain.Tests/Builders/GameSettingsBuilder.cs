using ConnectFour.Domain.GameDomain;

namespace ConnectFour.Domain.Tests.Builders;

public class GameSettingsBuilder
{
    private readonly GameSettings _settings;

    public GameSettingsBuilder()
    {
        _settings = new GameSettings
        {
            AutoMatchCandidates = true,
            EnablePopOut = false,
            ConnectionSize = 4,
            GridRows = 6,
            GridColumns = 7
        };
    }

    public GameSettingsBuilder WithAutoMatching(bool value)
    {
        _settings.AutoMatchCandidates = value;
        return this;
    }

    public GameSettingsBuilder WithGridDimensions(int numberOfRows, int numberOfColumns)
    {
        _settings.GridRows = numberOfRows;
        _settings.GridColumns = numberOfColumns;
        return this;
    }

    public GameSettingsBuilder WithConnectionSize(int size)
    {
        _settings.ConnectionSize = size;
        return this;
    }

    public GameSettingsBuilder WithPopOut(bool popOutEnabled)
    {
        _settings.EnablePopOut = popOutEnabled;
        return this;
    }

    public GameSettingsBuilder AsCopyOf(GameSettings otherSettings)
    {
        WithAutoMatching(otherSettings.AutoMatchCandidates);
        WithGridDimensions(otherSettings.GridRows, otherSettings.GridColumns);
        WithConnectionSize(otherSettings.ConnectionSize);
        WithPopOut(otherSettings.EnablePopOut);
        return this;
    }

    public GameSettings Build()
    {
        return _settings;
    }
}