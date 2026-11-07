using System;
using System.Collections;
using System.Collections.Generic;

namespace StSHsr.Battle
{
    public sealed class ContactDirector : IDisposable
    {
        private readonly BattleExecutor _executor;
        private readonly Dictionary<IBattleParticipant, IBattleParticipant> _declared =
            new Dictionary<IBattleParticipant, IBattleParticipant>();
        private readonly Dictionary<IBattleParticipant, Action<IBattleParticipant>> _handlers =
            new Dictionary<IBattleParticipant, Action<IBattleParticipant>>();

        public ContactDirector(BattleExecutor executor)
        {
            _executor = executor;
        }

        public void Register(IBattleParticipant participant)
        {
            if (_handlers.ContainsKey(participant))
            {
                return;
            }

            Action<IBattleParticipant> handler = target => _declared[participant] = target;
            _handlers[participant] = handler;
            participant.AttackDeclared += handler;
        }

        public void Unregister(IBattleParticipant participant)
        {
            if (_handlers.TryGetValue(participant, out var handler))
            {
                participant.AttackDeclared -= handler;
                _handlers.Remove(participant);
            }

            _declared.Remove(participant);
        }

        public IEnumerator OnContact(
            IBattleParticipant first,
            IBattleParticipant second,
            Action<BattleReport> onFinished = null)
        {
            bool firstAttacks = _declared.TryGetValue(first, out var firstTarget) && firstTarget == second;
            bool secondAttacks = _declared.TryGetValue(second, out var secondTarget) && secondTarget == first;

            yield return _executor.Run(first, second, firstAttacks, secondAttacks, onFinished);

            _declared.Remove(first);
            _declared.Remove(second);
        }

        public void Dispose()
        {
            foreach (var pair in _handlers)
            {
                pair.Key.AttackDeclared -= pair.Value;
            }

            _handlers.Clear();
            _declared.Clear();
        }
    }
}
