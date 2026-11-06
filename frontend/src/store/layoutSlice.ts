import { createSlice, PayloadAction } from '@reduxjs/toolkit';

interface LayoutState {
    sidebarOpen: boolean;
    activeStack: string[];
    pinnedStack: string[];
}

const initialState: LayoutState = {
    sidebarOpen: true,
    activeStack: [],
    pinnedStack: [],
};

const layoutSlice = createSlice({
    name: 'layout',
    initialState,
    reducers: {
        setSidebarOpen: (state, action: PayloadAction<boolean>) => {
            state.sidebarOpen = action.payload;
        },
        toggleSidebarState: (state) => {
            state.sidebarOpen = !state.sidebarOpen;
        },
        toggleDrawerState: (state, action: PayloadAction<string>) => {
            const id = action.payload;
            if (state.activeStack.includes(id)) {
                // If it's already active, we close it
                state.pinnedStack = state.pinnedStack.filter(t => t !== id);
                state.activeStack = state.activeStack.filter(t => t !== id);
                if (state.activeStack.length === 0 && state.pinnedStack.length === 0) {
                    state.sidebarOpen = true;
                }
            } else {
                state.sidebarOpen = false;
                state.activeStack.push(id);
            }
        },
        togglePinState: (state, action: PayloadAction<string>) => {
            const id = action.payload;
            if (state.pinnedStack.includes(id)) {
                state.pinnedStack = state.pinnedStack.filter(t => t !== id);
                if (state.pinnedStack.length === 0) {
                    state.sidebarOpen = true; // Re-open sidebar when last pin removed
                }
            } else {
                state.sidebarOpen = false; // Close sidebar when pinning a drawer
                if (!state.pinnedStack.includes(id)) {
                    state.pinnedStack.push(id);
                }
                // Bring to front of active stack
                state.activeStack = state.activeStack.filter(t => t !== id);
                state.activeStack.push(id);
            }
        },
        openPinnedItem: (state, action: PayloadAction<string>) => {
            const id = action.payload;
            // Bring to front of active stack
            state.activeStack = state.activeStack.filter(t => t !== id);
            state.activeStack.push(id);
            state.sidebarOpen = false;
        }
    },
});

export const { 
    setSidebarOpen, 
    toggleSidebarState, 
    toggleDrawerState, 
    togglePinState,
    openPinnedItem
} = layoutSlice.actions;

export default layoutSlice.reducer;
