import gym

from baselines import deepq
from baselines import logger

from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.envs.unity_gym_env import UnityToGymWrapper


def main():
    path = "..\\zibraai_core\\Build\\zibraai_core.exe"
    env = createUnityEnv(path)
    run(env)

def policy(observation, agent):
    return 0


def run(env):
    logger.configure('./logs')  # Change to log in a different directory
    act = deepq.learn(
        env # Change to save model in a different directory
    )
    print("Saving model to unity_model.pkl")
    act.save("unity_model.pkl")

def createUnityEnv(path: str):
    unity_env = UnityEnvironment(path)
    env = UnityToGymWrapper(unity_env, uint8_visual=True ) #, flatten_branched, allow_multiple_obs)
    env.reset()
    return env





if __name__ == '__main__':
    main()
