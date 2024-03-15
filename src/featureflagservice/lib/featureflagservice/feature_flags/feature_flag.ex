# Copyright The OpenTelemetry Authors
# SPDX-License-Identifier: Apache-2.0


defmodule Featureflagservice.FeatureFlags.FeatureFlag do
  use Ecto.Schema
  import Ecto.Changeset

  schema "featureflags" do
    field :description, :string
    field :enabled, :boolean, default: false
    field :name, :string
    field :value, :integer  # or :float, depending on your requirements

    timestamps()
  end

  @doc false
  def changeset(feature_flag, attrs) do
    feature_flag
    |> cast(attrs, [:name, :description, :enabled, :value])  # Include :value here
    |> validate_required([:name, :description, :enabled, :value])
    |> unique_constraint(:name)
  end
end
