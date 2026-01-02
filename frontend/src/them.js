import {create} from "zustand"

const useThemeStore = create((set,get)=>({
    mode:"light",
    changeMode:()=>set(()=> ({mode: get().mode === "light"? "dark": "light"}))
}))


export default useThemeStore
