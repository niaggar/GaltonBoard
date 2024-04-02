using GaltonBoard.Model.Configs;
using GaltonBoard.Model.Enums;
using GaltonBoard.Model.Models;

namespace GaltonBoard.Core.Utils;

public static class ParticlesFactory
{
    public static Particle[] CreateBalls(BallCreationConfig creationConfig, Border border)
    {
        var particles = new Particle[creationConfig.NumberOfBalls];

        for (var i = 0; i < creationConfig.NumberOfBalls; i++)
        {
            var fractionX = RandomUtils.NextDouble(creationConfig.CenterX);
            var fractionY = RandomUtils.NextDouble(creationConfig.CenterY);

            var angle = RandomUtils.NextDouble(creationConfig.VelocityAngleRange);
            var angleRadians = angle * Math.PI / 180;
            var fractionXVelocity = Math.Cos(angleRadians);
            var fractionYVelocity = Math.Sin(angleRadians);

            var initialPosition = new Vector(border.Width * fractionX, border.Height * fractionY);
            var initialVelocity = new Vector(creationConfig.VelocityMagnitude * fractionXVelocity, -creationConfig.VelocityMagnitude * fractionYVelocity);
            var mass = (float)RandomUtils.NextDouble(creationConfig.Mass);
            var particleConfig = new ParticleConfig()
            {
                Mass = mass,
                InverseMass = 1 / mass,
                Radius = (float)RandomUtils.NextDouble(creationConfig.Radio),
                Restitution = (float)creationConfig.Restitution,
                IsStatic = false
            };

            particles[i] = new Ball(particleConfig)
            {
                Position = initialPosition,
                Velocity = initialVelocity
            };
        }

        return particles;
    }

    public static Particle[] CreatePegs(PegCreationConfig creationConfig, BoardConfig boardConfig)
    {
        var pegs = new List<Peg>();
        var rows = boardConfig.NumberOfRows;
        var columns = boardConfig.NumberOfColumns;

        var positionsInX = new List<double>();
        var middleColumn = columns / 2;

        for (var i = 0; i < columns; i++) positionsInX.Add(i - (middleColumn));

        for (var i = 0; i < columns; i++)
        {
            var currentX = positionsInX[i];
            var finalX = currentX < 0
                ? currentX * boardConfig.ColumnsWidth / boardConfig.ResizeFactorX
                : currentX * boardConfig.ColumnsWidth * boardConfig.ResizeFactorX;

            positionsInX[i] = finalX;
        }

        var positionsInXWidthDisplacement = new List<double>();
        positionsInXWidthDisplacement.Add(positionsInX[0] - boardConfig.ColumnsWidth / 2);
        for (var i = 0; i < columns; i++)
            positionsInXWidthDisplacement.Add(positionsInX[i] + boardConfig.ColumnsWidth / 2);

        var mass = (float)RandomUtils.NextDouble(creationConfig.Mass);
        var particleConfig = new ParticleConfig()
        {
            Mass = mass,
            InverseMass = 1 / mass,
            Radius = (float)RandomUtils.NextDouble(creationConfig.Radio),
            Restitution = (float)creationConfig.Restitution,
            IsStatic = false
        };
        var pegConfig = new PegConfig()
        {
            XFrequency = creationConfig.XFrequency,
            YFrequency = creationConfig.YFrequency,
            XAmplitude = creationConfig.XAmplitude,
            YAmplitude = creationConfig.YAmplitude
        };

        var paticulaDePrueba = true;
        for (var i = 0; i < rows; i++)
        {
            var y = i * boardConfig.RowsHeight * boardConfig.ResizeFactorY + boardConfig.MarginDown;
            var rowIsEven = i % 2 == 0;
            var columnsToUse = rowIsEven ? positionsInX.Count : positionsInXWidthDisplacement.Count;

            for (var j = 0; j < columnsToUse; j++)
            {
                var x = rowIsEven ? positionsInX[j] : positionsInXWidthDisplacement[j];
                var peg = new Peg(particleConfig, pegConfig)
                {
                    Position = new Vector(x, y)
                };
                if (paticulaDePrueba)
                {
                    peg.Id = 6969696;
                    paticulaDePrueba = false;
                }

                pegs.Add(peg);
            }
        }

        return pegs.ToArray() as Particle[];
    }

    public static Particle[] CreateRectangularPegs(PegCreationConfig creationConfig, BoardConfig boardConfig)
    {
        var pegs = new List<Peg>();
        var rows = boardConfig.NumberOfRows;
        var columns = boardConfig.NumberOfColumns;

        var positionsInX = new List<double>();
        var middleColumn = columns / 2;

        for (var i = 0; i < columns; i++) positionsInX.Add(i - (middleColumn));

        for (var i = 0; i < columns; i++)
        {
            var currentX = positionsInX[i];
            var finalX = currentX < 0
                ? currentX * boardConfig.ColumnsWidth / boardConfig.ResizeFactorX
                : currentX * boardConfig.ColumnsWidth * boardConfig.ResizeFactorX;

            positionsInX[i] = finalX;
        }

        var mass = (float)RandomUtils.NextDouble(creationConfig.Mass);
        var particleConfig = new ParticleConfig()
        {
            Mass = mass,
            InverseMass = 1 / mass,
            Radius = (float)RandomUtils.NextDouble(creationConfig.Radio),
            Restitution = (float)creationConfig.Restitution,
            IsStatic = false
        };
        var pegConfig = new PegConfig()
        {
            XFrequency = creationConfig.XFrequency,
            YFrequency = creationConfig.YFrequency,
            XAmplitude = creationConfig.XAmplitude,
            YAmplitude = creationConfig.YAmplitude
        };

        for (var i = 0; i < rows; i++)
        {
            var y = i * boardConfig.RowsHeight * boardConfig.ResizeFactorY + boardConfig.MarginDown;
            var columnsToUse = positionsInX.Count;

            for (var j = 0; j < columnsToUse; j++)
            {
                var x = positionsInX[j];
                var peg = new Peg(particleConfig, pegConfig)
                {
                    Position = new Vector(x, y)
                };
                pegs.Add(peg);
            }
        }

        return pegs.ToArray() as Particle[];
    }
}