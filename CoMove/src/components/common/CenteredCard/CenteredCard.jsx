import { useNavigate } from "react-router-dom";
import logo from "../../../assets/kepek/logo/comove_logo1.png";
import { Flex, Paper, Image } from "@mantine/core";
import styles from './CenteredCard.module.css'

const blueInput = { input: { backgroundColor: "var(--lightbackground)" } };

function CenteredCard({ children, style = {}, maw = 500, m = '5%' }) {
    const navigate = useNavigate();

    return (
        <Flex mih='100vh' justify='center' align='center' bg='var(--lightbackground)'>
            <Paper p={40} radius='xl' w='100%' maw={maw} shadow='md' m={m} style={style}>
                <Image src={logo} w={45} h={45} onClick={() => navigate("/")} style={{ cursor: "pointer" }} mb={25} />
                {children}
            </Paper>
        </Flex>
    )
}

                    
export { styles, blueInput };
export default CenteredCard;