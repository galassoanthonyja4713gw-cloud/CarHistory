import { useMemo, useState } from "react";
import {
    useCarEntries,
    useCreateCarEntry,
    useDeleteCarEntry,
    useUpdateCarEntry,
} from "./features/carEntries/hooks";

function toInputDate(dateOnlyString) {
    return dateOnlyString ?? "";
}

export default function App() {
    const { data, isLoading, error } = useCarEntries();
    const create = useCreateCarEntry();
    const del = useDeleteCarEntry();
    const update = useUpdateCarEntry();

    const [createForm, setCreateForm] = useState({
        make: "",
        model: "",
        year: new Date().getFullYear(),
        purchaseDate: "",
        odometerAtPurchase: "",
    });

    const [editingId, setEditingId] = useState(null);
    const [editForm, setEditForm] = useState(null);

    const entries = useMemo(() => (Array.isArray(data) ? data : []), [data]);

    function onCreateChange(e) {
        const { name, value } = e.target;
        setCreateForm((f) => ({ ...f, [name]: value }));
    }

    async function onCreateSubmit(e) {
        e.preventDefault();
        await create.mutateAsync({
            make: createForm.make,
            model: createForm.model,
            year: Number(createForm.year),
            purchaseDate: createForm.purchaseDate,
            odometerAtPurchase: Number(createForm.odometerAtPurchase),
        });
        setCreateForm({
            make: "",
            model: "",
            year: new Date().getFullYear(),
            purchaseDate: "",
            odometerAtPurchase: "",
        });
    }

    function beginEdit(entry) {
        setEditingId(entry.id);
        setEditForm({
            make: entry.make ?? "",
            model: entry.model ?? "",
            trim: entry.trim ?? "",
            year: entry.year ?? new Date().getFullYear(),
            color: entry.color ?? "",
            hp: entry.hp ?? "",
            tq: entry.tq ?? "",
            vin: entry.vin ?? "",
            notes: entry.notes ?? "",
            purchaseDate: toInputDate(entry.purchaseDate),
            soldDate: toInputDate(entry.soldDate),
            odometerAtPurchase: entry.odometerAtPurchase ?? "",
            odometerAtSale: entry.odometerAtSale ?? "",
        });
    }

    function cancelEdit() {
        setEditingId(null);
        setEditForm(null);
    }

    function onEditChange(e) {
        const { name, value } = e.target;
        setEditForm((f) => ({ ...f, [name]: value }));
    }

    async function saveEdit() {
        const id = editingId;
        if (!id || !editForm) return;

        const payload = {
            make: editForm.make,
            model: editForm.model,
            trim: editForm.trim || null,
            year: Number(editForm.year),
            color: editForm.color || null,
            hp: editForm.hp === "" ? null : Number(editForm.hp),
            tq: editForm.tq === "" ? null : Number(editForm.tq),
            vin: editForm.vin || null,
            notes: editForm.notes || null,
            purchaseDate: editForm.purchaseDate,
            soldDate: editForm.soldDate || null,
            odometerAtPurchase: Number(editForm.odometerAtPurchase),
            odometerAtSale: editForm.odometerAtSale === "" ? null : Number(editForm.odometerAtSale),
        };

        await update.mutateAsync({ id, payload });
        cancelEdit();
    }

    return (
        <div style={{ maxWidth: 980, margin: "0 auto", padding: 16, fontFamily: "system-ui" }}>
            <h1 style={{ marginBottom: 8 }}>Car History</h1>

            <section style={{ border: "1px solid #ddd", borderRadius: 12, padding: 16, marginBottom: 16 }}>
                <h2 style={{ marginTop: 0 }}>Add entry</h2>
                <form onSubmit={onCreateSubmit} style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 12 }}>
                    <input name="make" placeholder="Make" value={createForm.make} onChange={onCreateChange} required />
                    <input name="model" placeholder="Model" value={createForm.model} onChange={onCreateChange} required />
                    <input name="year" type="number" placeholder="Year" value={createForm.year} onChange={onCreateChange} />
                    <input
                        name="purchaseDate"
                        placeholder="Purchase Date (YYYY-MM-DD)"
                        value={createForm.purchaseDate}
                        onChange={onCreateChange}
                        required
                    />
                    <input
                        name="odometerAtPurchase"
                        type="number"
                        placeholder="Odometer at purchase"
                        value={createForm.odometerAtPurchase}
                        onChange={onCreateChange}
                        required
                    />

                    <button type="submit" disabled={create.isPending} style={{ gridColumn: "1 / -1" }}>
                        {create.isPending ? "Saving..." : "Create"}
                    </button>

                    {create.isError && (
                        <div style={{ gridColumn: "1 / -1", color: "crimson" }}>{create.error.message}</div>
                    )}
                </form>
            </section>

            <section style={{ border: "1px solid #ddd", borderRadius: 12, padding: 16 }}>
                <h2 style={{ marginTop: 0 }}>Entries</h2>

                {isLoading && <div>Loading…</div>}
                {error && <div style={{ color: "crimson" }}>{error.message}</div>}

                {entries.length === 0 && !isLoading && <div>No entries yet.</div>}

                <div style={{ display: "grid", gap: 12 }}>
                    {entries.map((e) => {
                        const isEditing = editingId === e.id;

                        return (
                            <div key={e.id} style={{ border: "1px solid #eee", borderRadius: 12, padding: 12 }}>
                                {!isEditing ? (
                                    <div style={{ display: "flex", justifyContent: "space-between", gap: 12 }}>
                                        <div>
                                            <div style={{ fontWeight: 700 }}>
                                                {e.year} {e.make} {e.model} {e.trim || ""}
                                            </div>
                                            <div style={{ opacity: 0.8 }}>
                                                {e.color || "-"} Purchased: {e.purchaseDate}
                                                {e.soldDate ? ` Sold: ${e.soldDate}` : " Current"}
                                            </div>
                                            <div style={{ opacity: 0.8 }}>
                                                Odo: {e.odometerAtPurchase}
                                                {e.odometerAtSale ? ` -> ${e.odometerAtSale}` : ""}
                                                {"  "}Miles driven: {e.milesDriven ?? "-"}
                                                {"  "}Days owned: {e.ownershipDays}
                                            </div>
                                        </div>

                                        <div style={{ display: "flex", gap: 8 }}>
                                            <button onClick={() => beginEdit(e)} disabled={del.isPending || update.isPending}>
                                                Edit
                                            </button>
                                            <button onClick={() => del.mutate(e.id)} disabled={del.isPending || update.isPending}>
                                                Delete
                                            </button>
                                        </div>
                                    </div>
                                ) : (
                                    <div style={{ display: "grid", gap: 10 }}>
                                        <div style={{ fontWeight: 700 }}>Edit entry</div>

                                        <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 10 }}>
                                            <input name="make" value={editForm.make} onChange={onEditChange} placeholder="Make" />
                                            <input name="model" value={editForm.model} onChange={onEditChange} placeholder="Model" />
                                            <input name="trim" value={editForm.trim} onChange={onEditChange} placeholder="Trim" />
                                            <input name="year" type="number" value={editForm.year} onChange={onEditChange} placeholder="Year" />
                                            <input name="color" value={editForm.color} onChange={onEditChange} placeholder="Color" />
                                            <input name="vin" value={editForm.vin} onChange={onEditChange} placeholder="VIN" />
                                            <input name="hp" type="number" value={editForm.hp} onChange={onEditChange} placeholder="HP" />
                                            <input name="tq" type="number" value={editForm.tq} onChange={onEditChange} placeholder="TQ" />

                                            <input name="purchaseDate" value={editForm.purchaseDate} onChange={onEditChange} placeholder="Purchase Date (YYYY-MM-DD)" />
                                            <input name="soldDate" value={editForm.soldDate} onChange={onEditChange} placeholder="Sold Date (YYYY-MM-DD)" />

                                            <input name="odometerAtPurchase" type="number" value={editForm.odometerAtPurchase} onChange={onEditChange} placeholder="Odo at purchase" />
                                            <input name="odometerAtSale" type="number" value={editForm.odometerAtSale} onChange={onEditChange} placeholder="Odo at sale" />

                                            <textarea
                                                name="notes"
                                                value={editForm.notes}
                                                onChange={onEditChange}
                                                placeholder="Notes"
                                                style={{ gridColumn: "1 / -1", minHeight: 70 }}
                                            />
                                        </div>

                                        <div style={{ display: "flex", gap: 8 }}>
                                            <button onClick={saveEdit} disabled={update.isPending}>
                                                {update.isPending ? "Saving..." : "Save"}
                                            </button>
                                            <button onClick={cancelEdit} disabled={update.isPending}>
                                                Cancel
                                            </button>
                                            {update.isError && <div style={{ color: "crimson" }}>{update.error.message}</div>}
                                        </div>
                                    </div>
                                )}
                            </div>
                        );
                    })}
                </div>
            </section>
        </div>
    );
}
