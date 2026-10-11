# 第六期实现与待补素材

## 已接入

7 张牌均已注册稳定 ID、作者信息、角色卡池、独立百科池、公开 API、
默认开启的单卡设置，以及第六期折叠设置区的一键开启/关闭。
联机沿用前五期的房主设置快照与存档同步，不另建一套配置。

冠军为“神化？？？”。“戴冠”使用统一冠军目录，关闭此牌后会将其排除。

| 卡牌 | 卡池 | 基础 → 升级 |
| --- | --- | --- |
| 回滚 | 故障机器人 | 0 费；至多 2 张已升级手牌降级并附魔稳定；升级获得保留 |
| 仓皇 | 无色 | 0 费、固有；先将任意选中手牌降级，再打出；升级改为先打出选中的升级牌，再统一降级 |
| 最佳拍档 | 亡灵契约师 | 1 费；召唤 5 → 7；从所有战斗牌堆选择奥斯提攻击牌附魔完美契合 |
| 神化？？？ | 无色 | 1 费、固有；升级全部战斗卡牌；升级版本失去固有，手中时必须优先打出，并降级所有战斗卡牌 |
| 变数 | 静默猎手 | 1 费；交换两张非 X、可打出手牌的基础费用和局部修正，保留修正的到期条件；费用唯一更高者附魔蛇行；升级获得保留 |
| 自动武器 | 无色 | 1 费能力；抽牌堆至多 2 → 3 张攻击牌获得不能被打出；抽到不能被打出攻击牌时，通过正常攻击管线向敌人造成其伤害 |
| 超新星 | 储君 | 2 费；获得 3 → 4 星星；本回合将手牌所需能量转换为星星，保留原生星星费用，不永久修改卡牌费用 |

## 缺失素材

以下素材没有生成或伪造。目前卡牌、能力使用已有通用占位图，
百科使用第一期图标兜底；供应正式图片后会自动优先加载。
图片在此仓库按原有规则不纳入 Git，所需路径登记于资源清单。
第六期条目标记 optional，缺失时导出警告，存在时仍检查尺寸。

- `TheArchitectCup/images/cards/the_architect_cup_card_phase6_rollback.png`：1000 × 760
- `TheArchitectCup/images/cards/the_architect_cup_card_phase6_haste.png`：1000 × 760
- `TheArchitectCup/images/cards/the_architect_cup_card_phase6_best_partner.png`：1000 × 760
- `TheArchitectCup/images/cards/the_architect_cup_card_phase6_apotheosis.png`：1000 × 760
- `TheArchitectCup/images/cards/the_architect_cup_card_phase6_uncertainty.png`：1000 × 760
- `TheArchitectCup/images/cards/the_architect_cup_card_phase6_automatic_weapon.png`：1000 × 760
- `TheArchitectCup/images/cards/the_architect_cup_card_phase6_supernova.png`：1000 × 760
- `TheArchitectCup/images/powers/the_architect_cup_power_automatic_weapon_power.png`：64 × 64
- `TheArchitectCup/images/powers/big/the_architect_cup_power_automatic_weapon_power.png`：256 × 256
- `TheArchitectCup/images/powers/the_architect_cup_power_supernova_power.png`：64 × 64
- `TheArchitectCup/images/powers/big/the_architect_cup_power_supernova_power.png`：256 × 256
- `TheArchitectCup/images/ui/the_architect_cup_six.png`：64 × 64

## 验证方式

```powershell
pwsh -NoProfile -File tools/Validate-Localization.ps1
dotnet build -p:RunPckExport=false
pwsh -NoProfile -File tools/Test-Phase6.ps1 -Configuration Debug
dotnet build -c Release
pwsh -NoProfile -File tools/Test-Phase6.ps1 -Configuration Release
```

模型检查在独立进程中运行，验证 API/设置/百科/冠军元数据、基础与升级数值、
降级还原、超新星费用转换，以及 X 费所需星星。它不替代完整游戏实测。

### 待游戏内与双客户端实测

- 各角色奖励、商店、百科、单卡/批量开关；不同本地设置时以房主设置为准。
- 回滚：已附魔牌、零选择、空手牌、降级后保留行为。
- 仓皇：多牌播放顺序，前一张牌移动后续选中牌、击杀最后敌人时安全退出。
- 最佳拍档：没有符合条件的牌；抽牌堆/弃牌堆/消耗堆中的奥斯提攻击牌。
- 神化？？？：优先打出限制，自动打出不被拦截，降级自身后限制消失。
- 变数：相同费用、临时减费、附魔费用修正、已有附魔、X/不能被打出牌排除。
- 自动武器：力量、虚弱、易伤、敌方防御、奥斯提力量、抽牌击杀最后敌人。
- 超新星：普通/减费/零费/X 费牌、原生星星费用、自动打出、
  星星不足、多个玩家、重复施加、回合结束恢复以及星星费用显示。
- 戴冠：第六期冠军纳入候选；关闭冠军开关后不再生成。
