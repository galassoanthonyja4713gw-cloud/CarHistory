import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { api } from "../../api/client";

const keys = {
    all: ["carEntries"],
};

export function useCarEntries() {
    return useQuery({
        queryKey: keys.all,
        queryFn: () => api.get("/api/car-entries"),
    });
}

export function useCreateCarEntry() {
    const qc = useQueryClient();
    return useMutation({
        mutationFn: (payload) => api.post("/api/car-entries", payload),
        onSuccess: () => qc.invalidateQueries({ queryKey: keys.all }),
    });
}

export function useDeleteCarEntry() {
    const qc = useQueryClient();
    return useMutation({
        mutationFn: (id) => api.del(`/api/car-entries/${id}`),
        onSuccess: () => qc.invalidateQueries({ queryKey: keys.all }),
    });
}

export function useUpdateCarEntry() {
    const qc = useQueryClient();
    return useMutation({
        mutationFn: ({ id, payload }) => api.put(`/api/car-entries/${id}`, payload),
        onSuccess: () => qc.invalidateQueries({ queryKey: ["carEntries"] }),
    });
}

