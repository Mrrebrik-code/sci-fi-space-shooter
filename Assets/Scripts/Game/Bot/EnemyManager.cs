using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : SingletonMono<EnemyManager>
{
	public List<PoolControllerMono<AbstractBullet>> _pools;

	[SerializeField] private List<EnemyContainer> _containers;

	[SerializeField] private Enemy _enemyPrefab;
	[SerializeField] private Transform _enemysContent;


	[SerializeField] private AbstractBullet _bulletPrefab;
	[SerializeField] private Transform _bulletContent;

	
	private List<Enemy> _enemys = new List<Enemy>();

	private Action _onCallbackEndRoundEnemys;

	public void CreateEnemy(int round)
	{
		for (int i = 0; i < round; i++)
		{
			Enemy enemy = Instantiate(_enemyPrefab, _enemysContent);
			_enemys.Add(enemy);


			foreach (var container in _containers)
			{
				bool isAddEnemy = container.AddEnemy(enemy);

				if (isAddEnemy == true) break;
			}

			enemy.Init();
			enemy.SubscribeOnDied(OnHandleDiedBot);
			enemy.MoveToPath();
		}

		_pools = new List<PoolControllerMono<AbstractBullet>>();

		for (int i = 0; i < 1; i++)
		{
			var pool = new PoolControllerMono<AbstractBullet>(_bulletPrefab, 1, _bulletContent);
			_pools.Add(pool);
			_pools[i].IsAutoExpand = true;
		}

	}
	private void OnHandleDiedBot(Enemy enemy)
	{
		_enemys.Remove(enemy);

		if(_enemys.Count <= 0)
		{
			_onCallbackEndRoundEnemys?.Invoke();
		}
	}

	public void Subscribe(Action callbackEndRound)
	{
		_onCallbackEndRoundEnemys += callbackEndRound;
	}
}
