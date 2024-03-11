public enum MusicArea
{
    MAINMENU = 0,
    LEVEL_MUSIC = 1,
}

public enum VolumeType
{
    MASTER = 0,
    MUSIC = 10,
    AMBIENCE = 20,
    SFX = 30,
}

public enum SoundAreaType
{
    PLAYER,
    ENEMY,
}

public enum PlayerMovingState
{
    WALK = 0,
    SPRINT = 1,
    CROUCH = 2,
}

public enum EnemyAlertState
{
    ROAMING = 0,
    DISTRACTED = 1,
    AGGRESSIVE = 2,
}
