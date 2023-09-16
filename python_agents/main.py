# Suppress Warnings
# import warnings; warnings.filterwarnings("ignore")
# import os; os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3' 

# from mlagents_envs.environment import UnityEnvironment
# from mlagents_envs.envs.unity_gym_env import UnityToGymWrapper
from envs.boat_env import BoatNavigation
from basic_rl.main import BasicRL
from basic_rl.algorithms.REINFORCE import Reinforce
from basic_rl.algorithms.PPO import PPO
import numpy as np
import random
import torch
import time

def main():
	seed = 510
	print( "Hello World Boat Navigation!" )

	runBoatEnv('REINFORCE_100_500_UPDATE_5', 'REINFORCE', 100, 500, 5, seed, reward_shape_type='MUL')
	runBoatEnv('REINFORCE_100_500_UPDATE_5', 'REINFORCE', 100, 500, 5, seed, reward_shape_type='MUL')
	runBoatEnv('REINFORCE_100_500_UPDATE_5', 'REINFORCE', 100, 500, 5, seed, reward_shape_type='MUL')

	runBoatEnv('REINFORCE_100_500_UPDATE_10', 'REINFORCE', 100, 500, 10, seed, reward_shape_type='MUL')
	runBoatEnv('REINFORCE_100_500_UPDATE_10', 'REINFORCE', 100, 500, 10, seed, reward_shape_type='MUL')
	runBoatEnv('REINFORCE_100_500_UPDATE_10', 'REINFORCE', 100, 500, 10, seed, reward_shape_type='MUL')

	runBoatEnv('REINFORCE_100_500_UPDATE_15', 'REINFORCE', 100, 500, 15, seed, reward_shape_type='MUL')
	runBoatEnv('REINFORCE_100_500_UPDATE_15', 'REINFORCE', 100, 500, 15, seed, reward_shape_type='MUL')
	runBoatEnv('REINFORCE_100_500_UPDATE_15', 'REINFORCE', 100, 500, 15, seed, reward_shape_type='MUL')






def runBoatEnv(name, algorithm, num_episodes, step_limit, trajectory_update, random_seed, reward_shape_type=None, editor_run=False):
	random.seed(random_seed)
	np.random.seed(random_seed)
	torch.manual_seed(random_seed)
	env = BoatNavigation(step_limit=step_limit, editor_run=True, random_seed=random_seed, reward_shape_type=reward_shape_type)

	print(f"======================================= START: {name} =======================================")
	# Starting the training with PPO
	start_time = time.time()
	basic_rl = BasicRL(algorithm, env, verbose=2, seed=random_seed)
	if isinstance(basic_rl.algorithm, (PPO, Reinforce)):
		basic_rl.algorithm.trajectory_update = trajectory_update

	basic_rl.train(num_episode=num_episodes)
	total_time = time.time() - start_time

	print("Success: ", env.num_success)
	print("Failures: ", env.num_fail)

	print(f"---------------------------------------- END: {name} ----------------------------------------")

	env.close()
	return env.num_success, env.num_fail, total_time


if __name__ == "__main__":
	main()