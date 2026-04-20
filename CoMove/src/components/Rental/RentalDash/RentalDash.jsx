import { Paper } from "@mantine/core";
import { STATUS_DICT } from "../../../assets/scripts/Config";
import { useUser } from "../../../assets/scripts/AuthUser";
import OfferDash from "./OfferDash";
import InfoDash from "./InfoDash";
import StatusDash from "./StatusDash";
import RatingDash from "./RatingDash";

function Dash({ rental }) {
    const { data: authUser } = useUser();
    const status = STATUS_DICT[rental?.status];

    if (status.num < 2) {
        return <OfferDash rental={rental} me={authUser} />
    } else if (status.num === 8) {
        return <RatingDash rental={rental} me={authUser} />
    } else if (status.num === 9) {
        return <InfoDash rental={rental} />
    } else {
        return <StatusDash rental={rental} me={authUser} />
    }
}

function RentalDash({ rental }) {
    return (
        <Paper p='md' radius='md' shadow="0 8px 10px rgba(0, 0, 0, 0.48)">
            <Dash rental={rental} />
        </Paper>
    )
}

export default RentalDash;