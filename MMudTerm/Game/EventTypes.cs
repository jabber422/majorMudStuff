using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMudTerm.Game
{
    public enum EventType
    {
        Room,
        RoomLook,
        RoomSomethingMovedInto,
        RoomSomethingMovedOut,


        Combat,
        CombatEngaged,
        CombatEngagedStart,
        CombatEngagedStop,


        ExperienceGain,
        Tick,
        Top,
        Who,
        Stats,
        Inventory,
        DoorClosed,
        DoorOpen,
        SearchFound,
        DropCoins,
        PickUpCoins,
        HidCoins,
        HidItem,
        DropItem,
        PickUpItem,
        BoughtSomething,
        EntityDeath,
        None,
        HearMovement,
        SomeoneEnteredTheGame,
        Rest,
        StatsExperienceUpdate,
        SoldSomething,
        BuffSpellCastFail,
        BuffSpellCastFail_3rdP,
        BashDoorFailure,
        BashDoorSuccess,
        AlsoHere,
        BuffSpellCastSuccess,
        BuffSpellCastSuccess_3rdP,
        PVP_ATTACKED,
        CombatEngagedStart_3rd,
        BuffExpired,
        ShopList,
        SomeoneLeftTheGame,
        BadRoomMove,
        SearchNotice,
        SneakAttempt,
        ForSaleList,
        ExperienceUpdate,
        CombatEngaged_3rdP,
        CombatHit,
        CombatMiss,
        CombatHitYou,
        CombatHitPlayer,
        CombatMissPlayer,
        DoorStateChange,
        DoorLocked,
        TelepathRcvd,
        Gossip,
        BadRoomMoveClosedDoor,
        EquippedArmor,
        EquippedWeapon,
        MessagesThatMakeUsPauseWhileWalking,
        PlayerBlind,
        SpellBook,
        SeeHiddenItem,
    }
}
