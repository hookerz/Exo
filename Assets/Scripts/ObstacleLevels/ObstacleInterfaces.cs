using UnityEngine;
using System;

public enum ObstacleLevelState
{
    Activating,
    Active,
    Deactivating,
    Deactive
}

public struct ObstacleCompleteData
{
    //IObstacleLevel level;
    //float timeElapsed;
    //int retries;
}

public struct ObstacleFailedData
{
    //Vector3 position;
}

public interface IInOutAnimated
{
    float DefaultInTime { get; }
    float DefaultOutTime { get; }

    void StartActivation();
    void ActivationProgress(float progress);
    void Activated();

    void StartDeactivation();
    void DeactivationProgress(float progress);
    void Deactivated();
}

public interface IObstacleLevel : IInOutAnimated
{
    string Name { get; }
    ObstacleLevelState State { get; }
    Transform RespawnLocation { get; }
    float InAnimationLength { get; }
    float OutAnimationLength { get; }
        
    event Action<IObstacleLevel, ObstacleLevelState> StateChanged;
    event Action<IObstacleLevel, ObstacleCompleteData> Completed;
    event Action<IObstacleLevel, ObstacleFailedData> PlayerFailed;
}

public interface IPlayerVehicle
{
    Rigidbody RigidBody { get; }
    Transform GroundRootTransform { get; }
}


public delegate void ObstacleStateChangedDelegate(ICompletableObstacle o, CompletableObstacleState oldState, CompletableObstacleState newState);

public enum CompletableObstacleState
{
    NotActive, 
    Active,
    Complete,
    Failed,
}

public interface ICompletableObstacle
{
    GameObject GameObject { get; }
    CompletableObstacleState ObstacleState { get; }

    event ObstacleStateChangedDelegate StateChanged;

    void Activate();
    void Deactivate();
    void Reset();
}

public delegate void ObstacleProgressDelegate(IProgressiveCompletableObstacle obstacle);
public delegate void ObstacleProgressPercentDelegate(IProgressiveCompletableObstacle obstacle, float percent);

public interface IProgressiveCompletableObstacle : ICompletableObstacle
{
    event ObstacleProgressDelegate ProgressiveObstacleStarted;
    event ObstacleProgressPercentDelegate ProgressiveObstaclePercent;
    event ObstacleProgressDelegate ProgressiveObstacleCanceled;
    // NOTE: completion state is handled by ICompletableObstacle
}


public delegate void CompletableObstacleSequenceProgressDelegate(ICompletableObstacleSequence sequence, ICompletableObstacle prev, ICompletableObstacle next);
public delegate void CompletableObstacleSequenceResetDelegate(ICompletableObstacleSequence sequence);

public interface ICompletableObstacleSequence : IProgressiveCompletableObstacle
{
    int SequenceLength { get; }
    int CurrentIndex { get; }
    ICompletableObstacle CurrentObstacle { get; }
}

public interface ICompletableObstacleCollection : IProgressiveCompletableObstacle
{
    int CollectionLength { get; }
    int CompletedCount { get; }
}
