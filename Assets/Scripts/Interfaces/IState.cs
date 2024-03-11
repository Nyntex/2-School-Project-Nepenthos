public interface IState
{
    public void Enter();

    public void UpdateState();

    public void Exit(Enemy_StateTypes stateType);
}
