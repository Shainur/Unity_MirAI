﻿using Assets.MirAI.Models;
using UnityEngine;

namespace Assets.MirAI.Simulation {
    public static class CommandHandler {

        private static readonly AiModel _model = AiModel.Instance;
        private static Unit currentUnit;

        public static void Handle(Unit unit) {
            currentUnit = unit;
            var node = FindCurrentUnitNode();
            if (node == null)
                return;
            ExecuteCommand(node.Command);
        }

        private static void ExecuteCommand(int command) {
            var p1 = command & 0x7F000000;
            if (p1 == 0) return;
            var p2 = (command >> 4) & 0x0F;

            if (p1 == 1 || p1 == 2) {
                bool any = p2 == 0;
                bool enemy = p2 == 1;
                var nearest = FindNearestUnit(any, enemy);
                if (nearest != null) {
                    var divider = p1 == 1 ? 1 : -1;
                    var dx = (nearest.X - currentUnit.X) * divider;
                    var dy = (nearest.Y - currentUnit.Y) * divider;
                    var velocity = new Vector2(dx, dy).normalized;
                    currentUnit.Controller.SetUnitVelocity(velocity);
                }
            }
        }

        private static Unit FindNearestUnit(bool any, bool enemy) {
            Unit result = null;
            var minDistance = 9999999f;
            foreach (var unit in _model.Units) {
                if (unit.Id == currentUnit.Id) continue;
                if (any || ((unit.Team == currentUnit.Team) != enemy)) {
                    var curDistance = Distance(unit.X, unit.Y, currentUnit.X, currentUnit.Y);
                    if (curDistance < minDistance) {
                        minDistance = curDistance;
                        result = unit;
                    }
                }
            }
            return result;
        }

        private static float Distance(float x1, float y1, float x2, float y2) {
            var dx = x1 - x2;
            var dy = y1 - y2;
            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        public static bool CheckCondition(int command) {
            //TODO check condition-node for valid
            return true;
        }

        private static Node FindCurrentUnitNode() {
            var prog = _model.Programs.Find(x => x.Id == currentUnit.ProgramId);
            return ProgramManager.Run(prog);
        }
    }
}
