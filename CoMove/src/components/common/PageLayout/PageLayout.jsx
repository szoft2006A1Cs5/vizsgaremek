import { useState, useEffect, useLayoutEffect } from 'react';
import { Box, Container, Group, Text } from '@mantine/core';
import style from './PageLayout.module.css';

function PageLayout({ title, subtitle, rightContent, children }) {
    return (
        <>
            <Box className={style.hero}>
                <Container
                    className={style.heroContent}
                    size="xl"
                    px={{ base: 20, sm: 40 }}
                >
                    <Group justify="space-between" align="flex-end">
                        <div>
                            <Text className={style.title}>{title}</Text>
                            <Text className={style.subtitle}>{subtitle}</Text>
                        </div>
                        {rightContent}
                    </Group>
                </Container>
            </Box>

            <Box className={style.siteContentBox}>
                <Container size="xl" px={{ base: 16, sm: 40 }} className={style.siteContentContainer}>
                    {children}
                </Container>
            </Box>
        </>
    );
}

export default PageLayout;
