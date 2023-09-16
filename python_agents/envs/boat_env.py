import math
import os, gym; gym.logger.set_level(40)
from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.envs.unity_gym_env import UnityToGymWrapper
import numpy as np

class BoatNavigation( gym.Env ):
    
	def __init__( self, step_limit=5000, worker_id=0, editor_run=True, random_seed=None, reward_shape_type = None ):

		# General Setup
		random_seed = np.random.randint(0, 1000) if random_seed is None else random_seed
		# Load input properties
		self.step_limit = step_limit

		self.coords_base = (0,0) # lat, lng

		# If the env_path is given as input override the environment search
		if not editor_run: env_path = "envs/BoatSimulation/BoatSimulation.exe"
		# For the editor build force the path to None and the worker id to 0, 
		# assigned values for the editor build.
		else: env_path = None; worker_id = 0

		# Load the Environment
		unity_env = UnityEnvironment(env_path, worker_id=worker_id, seed=random_seed )

		# Convert the Unity Environment in a OpenAI Gym Environment, setting some flag 
		# according with the current setup (only one branch in output and no vision observation)
		self.env = UnityToGymWrapper( unity_env, flatten_branched=True )

		# Override the action  space of the wrapper
		self.action_space = self.env.action_space
		print("BoatNavigation:action_space:",self.action_space)

		# Override the state_size, the orginal version provide a 2-input format with delta_x
		# and delta_y of the lumen with respect to the center of the camera
		state_size = 5

		# Acoording to the previous line, we override the observation space
		# with two elements bounded in [-1, 1]
		self.observation_space = gym.spaces.Box(
			np.array([ 0 for _ in range( state_size )] ),
			np.array([ 1 for _ in range( state_size )] ),
			dtype=np.float64
		)

		# Initialize the counter for the maximum step counter
		# and the local variable for the reward computation
		self.step_counter = 0
		self.num_success = 0
		self.num_fail = 0
		self.reward_shape_type = reward_shape_type


	def reset( self, options=None, seed=None,  *args ):

		#
		self.step_counter = 0
		original_state = self.env.reset()
		
		# Fix the state removing the last item (i.e., distance)
		# and get the distance
		self.coords_base = (original_state[2], original_state[3])

		state, _ = self.parse_state( original_state )


		#
		return state, {}
	

	def step(self, action):

		# Call the step function of the OpenAI Gym class
		# print(action)
		state, unity_reward, terminated, info = self.env.step( action )
		#print([round(st,5) for st in state])
		# Fix the state removing the last item (i.e., distance)
		# and get the distance
		state, distance = self.parse_state( state )

		# Increase the step counter
		self.step_counter += 1

		# Initialize the variables to return
		truncated = False
		reward, info = 0, {}



		if self.step_counter >= self.step_limit:
			truncated=True




		# Basic reward
		if self.reward_shape_type == 'mul':
			reward = self.reward_shaping_mul(distance)
		elif self.reward_shape_type == 'add':
			reward = self.reward_shaping_add(distance)
		elif self.reward_shape_type == 'square':
			reward = -(distance*distance)
		else:
			reward = -distance

		reward *= 0.0001

		if distance <= 15:
			#print("+1 point for Gryffindor finding the red ball.")
			self.num_success += 1
			terminated = True
			reward += 1000

		# Special reward flag for the boat outside the
		# limit of the environment
		if unity_reward == -1:
			#print("-1 point for Gryffindor for leaving the field.")
			self.num_fail += 1
			terminated = True
			reward -= 1000

		return state, reward, terminated, truncated, info

	def reward_shaping_mul(self, distance):
		range = (20, 300)
		areas = 6
		multiplier = 2
		area_size = abs(range[1] - range[0]) // areas
		area_num = areas - (distance // area_size)
		if area_num < 0: area_num = 0
		reward = -(distance * (multiplier ** area_num))
		return reward


	def reward_shaping_add(self, distance):
		bonus = 0.001
		reward = -(distance)

		if distance < 75:
			reward += bonus

		if distance < 50:
			reward += bonus

		if distance < 25:
			reward += bonus

		return reward

	def parse_state( self, original_state ):
		
		# Extract raw values from the sensors
		engine_sensor1 = original_state[0]
		engine_sensor2 = original_state[1]
		latitude = original_state[2]
		longitude = original_state[3]
		altitude = original_state[4]
		compass = original_state[5]
		acc_x, acc_y, acc_z = original_state[6], original_state[7], original_state[8]
		gyro_x, gyro_y, gyro_z = original_state[9], original_state[10], original_state[11]
		sonar_collision = original_state[12]
		sonar_distance = original_state[13]

		# Extract the distance from the goal
		goal_distance = original_state[-1]

		# TODO: Normalize the values for the training
		latitude = latitude - self.coords_base[0]
		longitude = longitude - self.coords_base[1]
		#compass = compass / 360

		compass_rads = math.radians(compass)
		compass_sin = math.sin(compass_rads)
		compass_cos = math.cos(compass_rads)
		distance_rel = goal_distance / 200 # assuming a max distance of 200m

		# Compose the processed state
		state = np.array([distance_rel,compass_sin, compass_cos, latitude, longitude ])

		return state, goal_distance
	

	# Override the "close" function
	def close( self ): self.env.close()

	# Override the "render" function
	def render( self ):	pass