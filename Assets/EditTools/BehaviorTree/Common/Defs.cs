#if UNITY_EDITOR

namespace BTEditor
{
	public enum SuccessMode
	{
		none,
		all,
		any,
	}

	public enum FailureMode
	{
		none,
		all,
		any,
	}

	// 比较类型
	public enum CompareType
	{
		空 = 0,
		大于 = 1,
		小于 = 2,
		等于 = 3,
	}

	// 震屏类型
	public enum ShockScreenType
	{
		空 = 0,
		脚本指令震屏 = 1,
	}

	// 点名规则类型
	public enum SelectType
	{
		空 = 0,
		随机个数 = 1,
		指定个数 = 2,
		比例个数 = 3,
		全部 = 4,
	}

	// 运算符类型
	public enum OperatorType
	{
		空 = 0,
		取余 = 1,
		相等 = 2,
		大于 = 3,
		小于 = 4,
	}

	// 筛选目标范围规则
	public enum RangeType
	{
		空 = 0,
		圆形 = 1,
	}

	// 条件类型
	public enum ConditionType
	{
		空 = 0,
		属性 = 1,
		距离 = 2,
		伤害 = 3,
		职业 = 4,
		Buff = 5,
	}

	// 条件规则
	public enum ConditionRule
	{
		空 = 0,
		最高 = 1,
		最低 = 2,
		阈值以上 = 3,
		阈值以下 = 4,
	}

	// 元素类型枚举支持选择
	public enum QuestActiveElementType
	{
		出生点 = 1,
		复活点 = 2,
		机关 = 3,
		怪物组 = 4,
		触发器 = 5,
		楼梯点 = 6,
		动态阻挡 = 7,
		NPC = 8,
		采集物组 = 9,

		机关_传送点 = 3001,

		触发器_场景特效 = 5001,
	}

	public enum PosType
	{
		空 = 0,
		相对坐标 = 1,
		绝对坐标 = 2,
		点名位置 = 3,
		保存目标位置 = 4,
		相对角度坐标 = 5,
	}

	// 比较类型
    public enum RelativeType
    {
        空 = 0,
        True = 1,
        False = 2,
    }

	public enum TargetType
	{
		空 = 0,
		怪物 = 1,
		NPC = 2,
		采集物 = 3,
		机器人 = 4,
	}

	public enum SummonTargetType
	{
		空 = 0,
		怪物 = 1,
		机器人 = 2,
		采集物 = 3,
	}

	public enum EffectType
	{
		空 = 0,
		怪物特效 = 1,
		场景特效 = 2,
		调用特效 = 3,
	}

	public enum StorageType
	{
		空 = 0,
		多对一存储 = 1,
		一对一存储 = 2,
	}

	public enum RunSpeedType
	{
		空 = 0,
		RunMode = 1,
		RunOnceMode = 2,
		MoveMode = 3,
		MoveOnceMode = 4,
	}

	public enum HpType
	{
		空 = 0,
		设置血量 = 1,
		恢复血量 = 2,
		减少血量 = 3,
	}

	public enum ConditionalType
	{
		空 = 0,
		血量 = 1,
		距离 = 2,
		基础职业 = 3,
		伤害 = 4,
		Buff = 5,
		点名目标数量 = 6,
		性别 = 7,
		位置 = 8,
	}

	public enum CondTargetType
	{
		空 = 0,
		自己 = 1,
		目标 = 2,
	}

	public enum CompanionActionType
	{
		空 = 0,
		动作 = 1,
		泡泡 = 2,
	}

	public enum SkillCastType
	{
		空 = 0,
		点名目标 = 1,
		变量名 = 2,
		相对坐标 = 3,
		绝对坐标 = 4,
		相对角度 = 5,
		绝对角度 = 6,
		相对角度位置 = 7,
		组id = 8,
		当前目标 = 9,
	}

	public enum TargetsCondition
	{
		空 = 0,
		距离 = 1,
		血量 = 2,
		血量百分比 = 3,
	}

	public enum SelectRange
	{
		空 = 0,
		目标列表中 = 1,
		目标列表外 = 2,
		全部 = 3,
	}

	public enum SelectTarget
	{
		空 = 0,
		玩家 = 1,
		怪物 = 2,
		机器人 = 3,
		采集物 = 4,
		全部 = 5,
	}

	public enum SelectRelation
	{
		空 = 0,
		敌方 = 1,
		友方 = 2,
		队友 = 3,
		自己 = 4,
		全部 = 5,
	}

	public enum SelectCondition
	{
		空 = 0,
		距离 = 1,
		输出伤害百分比 = 2,
		职业 = 3,
		buff = 4,
		组id = 5,
		无 = 6,
	}

	#region Client

	public enum ClientSearchType
	{
		任务击杀目标 = 1,
		敌人 = 2,
		采集物 = 3,
		主人 = 4,
		队长 = 5,
		任务保护目标 = 6,
	}

	public enum ClienMoveType
	{
		采集物 = 1,
		其他 = 2,
	}

	public enum ClientCheckType
	{
		超出跟随目标距离 = 1,
		超出跟随队长距离 = 2,
		队长处于战斗状态 = 3,
		在跟随战斗距离内 = 4,
		主人处于战斗状态 = 5,
	}
	
	public enum ClientFollowType
    {
        队长 = 1,
        目标 = 2,
    }

	#endregion Client
}

#endif